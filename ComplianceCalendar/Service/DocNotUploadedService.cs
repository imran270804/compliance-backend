using System;
using System.Linq;
using System.Threading.Tasks;
using ComplianceCalendar.Models;
using ComplianceCalendar.Repository.IRepository;
using ComplianceCalendar.Services.EMailService;

namespace ComplianceCalendar.Services
{
    public class DocNotUploadedService : IDocNotUploadedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailSender _emailSender;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailRepository _emailRepository;

        public DocNotUploadedService(IEmailSender emailSender, IServiceProvider serviceProvider, INotificationRepository notificationRepository, IEmailRepository emailRepository)
        {
            _emailSender = emailSender;
            _serviceProvider = serviceProvider;
            _notificationRepository = notificationRepository;
            _emailRepository = emailRepository;
        }

        public async Task ProcessDocNotUploadedAsync()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var filings = await _emailRepository.GetPendingFilingsAsync();

                    foreach (var filing in filings)
                    {
                        var userFilings = await _emailRepository.GetUserFilingsByFilingIdAsync(filing.Id);
                        var employeeIds = userFilings.Select(uf => uf.EmployeeId).ToList();
                        var employees = await _emailRepository.GetEmployeesByIdsAsync(employeeIds);

                        foreach (var employee in employees)
                        {
                            string formattedDate = filing.DueDate.ToString("dd MMMM yyyy");

                            // Prepare email subject and body
                            var subject = "Reminder: Document Upload Required for Completed Filing";
                            var body = $"<p>Dear {employee.EmpName},</p>" +
                                        $"<p>This is a reminder that although you have marked the following filing as completed, " +
                                        $"the required documents have not yet been uploaded:</p>" +
                                        $"<p><strong>Filing Details:</strong></p>" +
                                        $"<ul>" +
                                        $"    <li><strong>Filing Name:</strong> {filing.Particulars}</li>" +
                                        $"    <li><strong>Completion Date:</strong> {formattedDate}</li>" +
                                        $"</ul>" +
                                        $"<p>Please upload the necessary documents at your earliest convenience to ensure the filing can be fully completed.</p>" +
                                        $"<p>Thank you for your cooperation.</p>";

                            // Send email
                            await _emailSender.SendEmailAsync(employee.Email, subject, body);

                            // Prepare notification
                            var notification = new Notification
                            {
                                EmpId = employee.EmployeeId,
                                NotificationBody = $"The filing '{filing.Particulars}' which was due on '{formattedDate}' has been marked as completed but the document has not been uploaded.",
                                IsRead = false,
                                NotificationType = "Doc Not Uploaded"
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
                // Log exception (consider using a logging framework)
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Consider logging the exception or handling it in a way that is appropriate for your application
            }
        }
    }
}
