using AutoMapper;
using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComplianceCalendar.Repository
{
    public class FilingsRepository : IFilingsRepository
    {
        private readonly APIContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;

        public FilingsRepository(APIContext context, IMapper mapper, INotificationRepository notificationRepository)
        {
            _context = context;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<object>> GetAdminFilingsAsync(int employeeId, int year)
        {
            // Validate the year parameter if necessary
            if (year < 2000 || year > 2100) // Adjust this range as needed
            {
                throw new ArgumentException("Invalid year specified.");
            }

            // Calculate the start and end dates for the financial year based on the provided year
            var financialYearStart = new DateTime(year, 4, 1, 0, 0, 0, DateTimeKind.Utc);
            var financialYearEnd = new DateTime(year + 1, 3, 31, 23, 59, 59, DateTimeKind.Utc);

            // Retrieve the department IDs associated with the employee
            var departmentIds = await _context.AssignedToDept
                .Where(at => at.EmpId == employeeId)
                .Select(at => at.DepartmentId)
                .ToListAsync();

            // Retrieve the department names for the department IDs
            var departmentNames = await _context.Departments
                .Where(d => departmentIds.Contains(d.Id))
                .Select(d => d.DepName)
                .ToListAsync();

            // Query filings based on the department names and financial year
            var filingsOfAdmin = await _context.Filings
                .Include(f => f.CreatedBy)
                .Where(f => departmentNames.Contains(f.DepName) &&
                            f.DueDate >= financialYearStart &&
                            f.DueDate <= financialYearEnd)
                .Select(f => new
                {
                    FilingId = f.Id,
                    f.DueDate,
                    f.StatuteOrAct,
                    f.FormChallan,
                    f.Particulars,
                    f.Status,
                    f.DepName,
                    f.DocIsUploaded,
                    f.Remarks,
                    f.Review,
                    AssignedTo = _context.UserFilings
                        .Where(uf => uf.FilingId == f.Id)
                        .Select(uf => new
                            {
                                uf.Employee.EmpName
                            })
                        .ToList(),
                    AssignedToId = _context.UserFilings
                        .Where(uf => uf.FilingId == f.Id)
                        .Select(uf => new
                        {
                            uf.Employee.EmployeeId
                        })
                        .ToList()
                })
                .ToListAsync();

            return filingsOfAdmin;
        }


        public async Task<Filings> GetFilingByIdAsync(int id)
        {
            return await _context.Filings.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<bool> UpdateFilingStatusAsync(int id, UpdateFilingStatusDTO updateDTO)
        {
            var filingToUpdate = await _context.Filings.FindAsync(id);

            if (filingToUpdate == null)
            {
                return false;
            }

            // Use AutoMapper to map the DTO to the entity
            _mapper.Map(updateDTO, filingToUpdate);
            _context.Filings.Update(filingToUpdate);

            // Save changes to the database
            var saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0 && !string.IsNullOrEmpty(updateDTO.Remarks))
            {
                // Step 1: Get the employee ID mapped to the filing
                var employeeId = await _context.UserFilings
                    .Where(uf => uf.FilingId == id)
                    .Select(uf => uf.EmployeeId)
                    .FirstOrDefaultAsync();

                if (employeeId == 0)
                {
                    return true; // No employee found, no notification needed
                }

                // Step 2: Get the department ID of that employee
                var departmentId = await _context.Employees
                    .Where(e => e.EmployeeId == employeeId)
                    .Select(e => e.DepId)
                    .FirstOrDefaultAsync();

                if (departmentId == 0)
                {
                    return true; // No department found, no notification needed
                }

                // Step 3: Get admin employee IDs for the department
                var adminIds = await _context.AssignedToDept
                    .Where(atd => atd.DepartmentId == departmentId)
                    .Select(atd => atd.EmpId)
                    .ToListAsync();

                // Step 4: Get email addresses of the admins
                var adminEmails = await _context.Employees
                    .Where(e => adminIds.Contains(e.EmployeeId))
                    .Select(e => new
                    {
                        e.EmployeeId,
                        e.Email,
                        e.EmpName
                    })
                    .ToListAsync();

                var notificationBody = $"Remarks have been posted for the filing {filingToUpdate.Particulars} due on {filingToUpdate.DueDate.AddDays(1).ToString("dd MMMM yyyy")}. Please review the details.";

                // Create and save notifications
                foreach (var admin in adminEmails)
                {
                    var notification = new Notification
                    {
                        EmpId = admin.EmployeeId,
                        NotificationBody = notificationBody,
                        IsRead = false,
                        NotificationType = "Filing Remarks"
                    };

                    await _notificationRepository.AddNotificationAsync(notification);
                }

                await _notificationRepository.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> ReassignFilingAsync(int filingId, ReassignFilingsDTO reassignDTO, int employeeId)
        {
            try
            {
                var filingToReassign = await GetFilingByIdAsync(filingId);
                if (filingToReassign == null)
                {
                    return false;
                }
                _mapper.Map(reassignDTO, filingToReassign);
                _context.Filings.Update(filingToReassign);
                var saveResult = await _context.SaveChangesAsync();
                
                var reassignedFiling = await _context.UserFilings.FirstOrDefaultAsync(uf => uf.FilingId == filingId);
                if (reassignedFiling == null)
                {
                    return false;
                }
                reassignedFiling.EmployeeId = employeeId;
                var saveEmployeeChange = await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> FilingExistsAsync(int id)
        {
            return await _context.Filings.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<object>> GetUserFilingsAsync(int employeeId, int year)
        {
            // Calculate the start and end dates for the financial year based on the provided year
            var financialYearStart = new DateTime(year, 4, 1, 0, 0, 0, DateTimeKind.Utc);
            var financialYearEnd = new DateTime(year + 1, 3, 31, 23, 59, 59, DateTimeKind.Utc);

            var userFilings = await _context.UserFilings
                .Where(uf => uf.EmployeeId == employeeId &&
                             uf.Filings.DueDate >= financialYearStart &&
                             uf.Filings.DueDate <= financialYearEnd)
                .Select(uf => new
                {
                    FilingId = uf.Filings.Id,
                    uf.Filings.DueDate,
                    uf.Filings.StatuteOrAct,
                    uf.Filings.FormChallan,
                    uf.Filings.Particulars,
                    uf.Filings.Status,
                    uf.Filings.DepName,
                    uf.Filings.DocIsUploaded,
                    uf.Filings.Remarks,
                    uf.Filings.Review,
                    CreatedBy = uf.Filings.CreatedBy.EmpName,
                    AssignedTo = _context.UserFilings
                        .Where(innerUf => innerUf.FilingId == uf.FilingId)
                        .Select(innerUf => innerUf.Employee.EmpName)
                        .ToList()
                })
                .ToListAsync();

            return userFilings;
        }

        public async Task<bool> AddFilingAsync(Filings filing)
        {
            if (filing == null) return false;

            _context.Filings.Add(filing);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddUserFilingsAsync(IEnumerable<UserFilings> userFilings)
        {
            if (userFilings == null || !userFilings.Any()) return false;

            _context.UserFilings.AddRange(userFilings);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<DepartmentEmployeeDTO>> GetDepartmentEmployeeDTOsAsync(CancellationToken ct)
        {
            return await _context.Departments
                .Select(d => new DepartmentEmployeeDTO
                {
                    Id = d.Id,
                    DepName = d.DepName,
                    Employees = _context.Employees
                        .Where(e => e.DepId == d.Id)
                        .Select(e => new Employee
                        {
                            EmployeeId = e.EmployeeId,
                            EmpName = e.EmpName
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<string> ReviewFilingAsync(int filingId, ReviewDTO reviewDTO)
        {
            // Find the filing by its ID
            var filing = await _context.Filings.FirstOrDefaultAsync(f => f.Id == filingId);
            if (filing == null)
            {
                throw new KeyNotFoundException("Filing not found.");
            }

            // Check if the review is null
            if (string.IsNullOrEmpty(reviewDTO.Review))
            {
                throw new ArgumentException("Review cannot be null or empty.");
            }

            filing.Review = reviewDTO.Review;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Refresh the entity from the database
                await _context.Entry(filing).ReloadAsync();

                return "Review added successfully.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
