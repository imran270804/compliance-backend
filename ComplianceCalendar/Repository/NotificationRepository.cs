using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComplianceCalendar.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly APIContext _dbContext;

        public NotificationRepository(APIContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _dbContext.Notifications.AddAsync(notification);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<NotificationDTO>> GetUnreadNotificationsByEmpIdAsync(int empId)
        {
            return await _dbContext.Notifications
                .Where(n => n.EmpId == empId && !n.IsRead)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,
                    NotificationBody = n.NotificationBody,
                    NotificationType = n.NotificationType
                })
                .ToListAsync();
        }


        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _dbContext.Notifications.FindAsync(id);
        }

        public async Task MarkAsReadAsync(Notification notification)
        {
            notification.IsRead = true;
            _dbContext.Notifications.Update(notification);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int?> GetDepartmentIdByEmployeeIdAsync(int empId)
        {
            return await _dbContext.Employees
                .Where(e => e.EmployeeId == empId)
                .Select(e => e.DepId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<int>> GetEmployeeIdsByDepartmentIdAsync(int depId)
        {
            return await _dbContext.AssignedToDept
                .Where(a => a.DepartmentId == depId)
                .Select(a => a.EmpId)
                .ToListAsync();
        }
    }
}
