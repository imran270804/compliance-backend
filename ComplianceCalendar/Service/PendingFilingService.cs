using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ComplianceCalendar.Models;
using ComplianceCalendar.Data;
using ComplianceCalendar.Services.EMailService;
using ComplianceCalendar.Repository.IRepository;

namespace ComplianceCalendar.Services
{
    public class PendingFilingsService : IPendingFilingsService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailSender _emailSender;
        private readonly INotificationRepository _notificationRepository;

        public PendingFilingsService(IEmailSender emailSender, IServiceProvider serviceProvider, INotificationRepository notificationRepository)
        {
            _emailSender = emailSender;
            _serviceProvider = serviceProvider;
            _notificationRepository = notificationRepository;
        }

        public async Task ProcessPendingFilingsAsync()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _dbContext = scope.ServiceProvider.GetRequiredService<APIContext>();

                    // Get all filings with pending status
                    var filings = await _dbContext.Set<Filings>()
                        .Where(f => f.Status == "Pending")
                        .ToListAsync();

                    // Notify each employee about their pending filings
                    foreach (var filing in filings)
                    {
                        DateTime dueDate = filing.DueDate;
                        string formattedDate = dueDate.ToString("dd MMMM yyyy");

                        var employeeIds = await _dbContext.Set<UserFilings>()
                            .Where(uf => uf.FilingId == filing.Id)
                            .Select(uf => uf.EmployeeId)
                            .ToListAsync();

                        var employees = await _dbContext.Set<Employee>()
                            .Where(e => employeeIds.Contains(e.EmployeeId))
                            .ToListAsync();

                        foreach (var employee in employees)
                        {
                            var subject = "Reminder: Overdue Filing Notification";
                            var body = $"<p>Dear {employee.EmpName},</p>" +
                                       $"<p>This is a notification that the due date for the following assigned filing has passed and it remains incomplete:</p>" +
                                       $"<p><strong>Filing Details:</strong></p>" +
                                       $"<ul>" +
                                       $"    <li><strong>Filing Name:</strong> {filing.Particulars}</li>" +
                                       $"    <li><strong>Due Date:</strong> {formattedDate}</li>" +
                                       $"</ul>" +
                                       $"<p>Please address this overdue filing as soon as possible.</p>" +
                                       $"<p>We appreciate your prompt attention to this matter.</p>";

                            await _emailSender.SendEmailAsync(employee.Email, subject, body);

                            var notification = new Notification
                            {
                                EmpId = employee.EmployeeId,
                                NotificationBody = $"The filing '{filing.Particulars}' which was due on '{formattedDate}' is Pending.",
                                IsRead = false,
                                NotificationType = "Pending Filing"
                            };

                            // Save notification
                            await _notificationRepository.AddNotificationAsync(notification);
                            await _notificationRepository.SaveChangesAsync();
                        }
                    }

                    // Step 1: Get all admin IDs
                    var adminIds = await _dbContext.Set<AssignedTo>()
                        .Select(at => at.EmpId)
                        .Distinct()
                        .ToListAsync();

                    // Step 2: Get employees with their department IDs
                    var employeeDepartmentEmails = await _dbContext.Set<Employee>()
                        .Where(e => adminIds.Contains(e.EmployeeId))
                        .Select(e => new
                        {
                            EmployeeId = e.EmployeeId,
                            DepartmentIds = _dbContext.Set<AssignedTo>()
                                .Where(at => at.EmpId == e.EmployeeId)
                                .Select(at => at.DepartmentId)
                                .ToList(),
                            Email = e.Email,
                            EmpName = e.EmpName
                        })
                        .ToListAsync();

                    // Step 3: Get distinct department IDs
                    var departmentIds = employeeDepartmentEmails
                        .SelectMany(e => e.DepartmentIds)
                        .Distinct()
                        .ToList();

                    // Step 4: Get department names
                    var departments = await _dbContext.Set<Department>()
                        .Where(d => departmentIds.Contains(d.Id))
                        .ToDictionaryAsync(d => d.Id, d => d.DepName);

                    // Step 5: Check for pending filings and send emails to admins
                    foreach (var employee in employeeDepartmentEmails)
                    {
                        // Get department names for the employee
                        var depNames = employee.DepartmentIds
                            .Select(id => departments.TryGetValue(id, out var name) ? name : "Unknown")
                            .ToList();

                        // Get pending filings for the employee's departments
                        var pendingFilings = await _dbContext.Set<Filings>()
                            .Where(f => depNames.Contains(f.DepName) && f.Status == "Pending")
                            .ToListAsync();

                        // If there are pending filings, send a single email
                        if (pendingFilings.Any())
                        {
                            var pendingCount = pendingFilings.Count;
                            var filingDetails = string.Join("\n", pendingFilings.Select(f => $"Filing Name: {f.Particulars}\nAssigned To: {employee.EmpName}\nDue Date: {f.DueDate:dd MMMM yyyy}"));

                            var subject = "Notification of Overdue Filings";
                            var body = $"<p>Dear {employee.EmpName},</p>" +
                                       $"<p>This is a notification that the following filings assigned within your department are past their due dates and remain incomplete:</p>" +
                                       $"<p><strong>Overdue Filing Details:</strong></p>" +
                                       $"<p>{filingDetails}</p>" +
                                       $"<p>Please ensure that these filings are addressed as soon as possible.</p>" +
                                       $"<p>Thank you for your prompt attention to this matter.</p>";

                            await _emailSender.SendEmailAsync(employee.Email, subject, body);

                            var notification = new Notification
                            {
                                EmpId = employee.EmployeeId,
                                NotificationBody = $"There are {pendingCount} pending filings in your department. Please check overview for further details",
                                IsRead = false,
                                NotificationType = "Pending Filing"
                            };

                            // Save notification
                            await _notificationRepository.AddNotificationAsync(notification);
                            await _notificationRepository.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
