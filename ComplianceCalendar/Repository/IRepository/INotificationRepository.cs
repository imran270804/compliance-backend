using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplianceCalendar.Repository.IRepository
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task SaveChangesAsync();
        Task<List<NotificationDTO>> GetUnreadNotificationsByEmpIdAsync(int empId);
        Task<Notification> GetNotificationByIdAsync(int id);
        Task MarkAsReadAsync(Notification notification);
        Task<int?> GetDepartmentIdByEmployeeIdAsync(int empId);
        Task<List<int>> GetEmployeeIdsByDepartmentIdAsync(int depId);
    }
}
