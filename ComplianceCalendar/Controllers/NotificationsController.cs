using ComplianceCalendar.Models;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplianceCalendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationRepository notificationRepository, ILogger<NotificationController> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostFilingAssignedNotification(int empId, string particulars, DateTime dueDate)
        {
            try
            {
                var notification = new Notification
                {
                    NotificationType = "FilingAssigned",
                    NotificationBody = $"A new filing {particulars} has been assigned to you for {dueDate:dd MMMM yyyy}",
                    EmpId = empId,
                    IsRead = false
                };

                await _notificationRepository.AddNotificationAsync(notification);
                await _notificationRepository.SaveChangesAsync();

                return Ok(new { message = "Notification created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a notification");
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                }
                return StatusCode(500, new { message = "An error occurred while creating the notification. Please check the logs for more details." });
            }
        }

        [HttpPost("ClosedFiling")]
        public async Task<IActionResult> PostClosedNotification(int empId, string particulars, DateTime dueDate)
        {
            try
            {
                // Get department ID of the given employee
                var depId = await _notificationRepository.GetDepartmentIdByEmployeeIdAsync(empId);
                if (depId == null)
                {
                    return BadRequest(new { message = "Employee not found or employee does not belong to any department." });
                }

                // Get all employee IDs assigned to that department
                var employeeIds = await _notificationRepository.GetEmployeeIdsByDepartmentIdAsync(depId.Value);

                // Create notifications for all employees in the department
                var notifications = employeeIds.Select(eId => new Notification
                {
                    NotificationType = "FilingClosed",
                    NotificationBody = $"The filing {particulars} due on {dueDate:dd MMMM yyyy} has been closed",
                    EmpId = eId,
                    IsRead = false
                }).ToList();

                foreach (var notification in notifications)
                {
                    await _notificationRepository.AddNotificationAsync(notification);
                }

                await _notificationRepository.SaveChangesAsync();

                return Ok(new { message = "Notifications created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating notifications");
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                }
                return StatusCode(500, new { message = "An error occurred while creating notifications. Please check the logs for more details." });
            }
        }

        [HttpGet("unread/{empId}")]
        public async Task<IActionResult> GetUnreadNotifications(int empId)
        {
            try
            {
                var notifications = await _notificationRepository.GetUnreadNotificationsByEmpIdAsync(empId);

                if (notifications == null || notifications.Count == 0)
                {
                    return NotFound(new { message = "No unread notifications found for this employee" });
                }

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching unread notifications");
                return StatusCode(500, new { message = "An error occurred while fetching unread notifications. Please check the logs for more details." });
            }
        }

        [HttpPost("markasread/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var notification = await _notificationRepository.GetNotificationByIdAsync(id);
                if (notification == null)
                {
                    return NotFound(new { message = "Notification not found" });
                }

                await _notificationRepository.MarkAsReadAsync(notification);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking the notification as read");
                return StatusCode(500, new { message = "An error occurred while marking the notification as read. Please check the logs for more details." });
            }
        }
    }
}
