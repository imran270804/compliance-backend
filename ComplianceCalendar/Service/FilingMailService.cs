using System;
using System.Linq;
using System.Threading.Tasks;
using ComplianceCalendar.Models;
using ComplianceCalendar.Repository.IRepository;
using ComplianceCalendar.Services.EMailService;

namespace ComplianceCalendar.Services
{
    public class FilingMailService : IFilingMailService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailSender _emailSender;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailRepository _emailRepository;

        public FilingMailService(IEmailSender emailSender, IServiceProvider serviceProvider, INotificationRepository notificationRepository, IEmailRepository emailRepository)
        {
            _emailSender = emailSender;
            _serviceProvider = serviceProvider;
            _notificationRepository = notificationRepository;
            _emailRepository = emailRepository;
        }

        public async Task ProcessUpcomingFilingsAsync()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var startDate = DateTimeOffset.UtcNow;
                    var endDate = DateTimeOffset.UtcNow.AddDays(5);

                    // Get filings due within 5 days with status "Open"
                    var filings = await _emailRepository.GetFilingsDueSoonAsync(startDate, endDate, "Open");

                    foreach (var filing in filings)
                    {
                        DateTime dueDate = filing.DueDate;
                        DateTime endDateForFiling = DateTime.Now;

                        TimeSpan difference = dueDate - endDateForFiling;
                        int daysLeft = (int)difference.TotalDays;
                        daysLeft = Math.Max(daysLeft, 0);

                        var userFilings = await _emailRepository.GetUserFilingsByFilingIdAsync(filing.Id);
                        var employeeIds = userFilings.Select(uf => uf.EmployeeId).ToList();
                        var employees = await _emailRepository.GetEmployeesByIdsAsync(employeeIds);

                        foreach (var employee in employees)
                        {
                            var subject = "Reminder: Notification of Upcoming File Processing";
                            var body = $"<p>Dear {employee.EmpName},</p>" +
                                      $"<p>This is a friendly reminder that the due date for your assigned filing is approaching soon.</p>" +
                                      $"<p><strong>Filing Details:</strong></p>" +
                                      $"<ul>" +
                                      $"    <li><strong>Filing Name:</strong> {filing.Particulars}</li>" +
                                      $"    <li><strong>Due Date:</strong> {filing.DueDate.ToShortDateString()}</li>" +
                                      $"</ul>" +
                                      $"<p>Please ensure that the filing is completed by the specified due date.</p>" +
                                      $"<p>Thank you for your attention to this matter.</p>";

                            await _emailSender.SendEmailAsync(employee.Email, subject, body);

                            var notification = new Notification
                            {
                                EmpId = employee.EmployeeId,
                                NotificationBody = $"The filing '{filing.Particulars}' is due in '{daysLeft}' days and will expire on '{filing.DueDate.ToShortDateString()}'.",
                                IsRead = false,
                                NotificationType = "Imminent Filing"
                            };

                            await _notificationRepository.AddNotificationAsync(notification);
                            await _notificationRepository.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception (consider using a logging framework)
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Consider logging the exception or handling it in a way that is appropriate for your application
            }
        }
    }
}
