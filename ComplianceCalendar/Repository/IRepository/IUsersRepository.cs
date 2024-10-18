using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceCalendar.Repository.IRepository
{
    public interface IUsersRepository
    {
        public ActionResult<IEnumerable<object>> GetUsersAsync();
        public ActionResult<IEnumerable<object>> GetUsersAsync(int id);
        public IResult UpdateStatusAsync(int id, int adminId);
        public Task<ActionResult<IEnumerable<object>>> GetUsersByDepartmentAsync(string depName);
        /*public Task<IActionResult> AddUserToEmployeeTableAsync([FromBody] ActiveDirectoryUserDto userDto);*/
        public IResult DeleteAsync(int id);
        Task<ActionResult<Employee>> AddUserToEmployeeTableAsync(ActiveDirectoryUserDto userDto);

    }
}
