using AutoMapper;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repository.IRepository;
using ComplianceCalendar.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplianceCalendar.Repository;

namespace ComplianceCalendar.Services
{
    public class FilingsService : IFilingsService
    {
        private readonly IFilingsRepository _filingsRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly APIContext _context;
        private readonly IMapper _mapper;

        public FilingsService(IFilingsRepository filingsRepository, APIContext context, IMapper mapper, INotificationRepository notificationRepository)
        {
            _filingsRepository = filingsRepository;
            _context = context;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<object>> GetAdminFilingsAsync(int employeeId, int year)
        {
            return await _filingsRepository.GetAdminFilingsAsync(employeeId, year);
        }

        public async Task<IEnumerable<object>> GetUserFilingsAsync(int employeeId, int year)
        {
            if (year < 2000 || year > 2100)
            {
                throw new ArgumentException("Invalid year specified.");
            }

            return await _filingsRepository.GetUserFilingsAsync(employeeId, year);
        }

        public async Task<bool> AddFilingsAsync(AddFilingsDTO addFilingsDTO)
        {
            if (addFilingsDTO == null)
            {
                throw new ArgumentNullException(nameof(addFilingsDTO), "addFilingsDTO cannot be null");
            }

            if (addFilingsDTO.AssignedToList == null || !addFilingsDTO.AssignedToList.Any())
            {
                throw new ArgumentException("AssignedToList cannot be null or empty", nameof(addFilingsDTO.AssignedToList));
            }

            bool isRecurring = false;
            var filingsToCreate = new List<Filings>();

            if (addFilingsDTO.Recurrence == "No recurrence")
            {
                // Create a single filing
                isRecurring = false;
                filingsToCreate.Add(new Filings
                {
                    StatuteOrAct = addFilingsDTO.StatuteOrAct,
                    FormChallan = addFilingsDTO.FormChallan,
                    Particulars = addFilingsDTO.Particulars,
                    DueDate = addFilingsDTO.DueDate.AddDays(1),
                    DepName = addFilingsDTO.DepName,
                    CreatedById = addFilingsDTO.CreatedById,
                    CreatedDate = DateTime.UtcNow,
                    Status = "Open",
                    DocIsUploaded = false,
                    IsRecurring = isRecurring
                });
            }
            else if (addFilingsDTO.Recurrence == "1 month")
            {
                // Create 12 filings, one for each month of the next year
                isRecurring = true;
                var startDate = addFilingsDTO.DueDate;

                for (int i = 0; i < 36; i++)
                {
                    filingsToCreate.Add(new Filings
                    {
                        StatuteOrAct = addFilingsDTO.StatuteOrAct,
                        FormChallan = addFilingsDTO.FormChallan,
                        Particulars = addFilingsDTO.Particulars,
                        DueDate = startDate.AddMonths(i), // Set due date to each month
                        DepName = addFilingsDTO.DepName,
                        CreatedById = addFilingsDTO.CreatedById,
                        CreatedDate = DateTime.UtcNow,
                        Status = "Open",
                        DocIsUploaded = false,
                        IsRecurring = isRecurring
                    });
                }
            }
            else if (addFilingsDTO.Recurrence == "3 months")
            {

                isRecurring = true;
                var startDate = addFilingsDTO.DueDate;

                for (int i = 0; i < 12; i++)
                {
                    filingsToCreate.Add(new Filings
                    {
                        StatuteOrAct = addFilingsDTO.StatuteOrAct,
                        FormChallan = addFilingsDTO.FormChallan,
                        Particulars = addFilingsDTO.Particulars,
                        DueDate = startDate.AddMonths(i*3), // Set due date to each quarter
                        DepName = addFilingsDTO.DepName,
                        CreatedById = addFilingsDTO.CreatedById,
                        CreatedDate = DateTime.UtcNow,
                        Status = "Open",
                        DocIsUploaded = false,
                        IsRecurring = isRecurring
                    });
                }
            }
            else if (addFilingsDTO.Recurrence == "6 months")
            {

                isRecurring = true;
                var startDate = addFilingsDTO.DueDate;

                for (int i = 0; i < 6; i++)
                {
                    filingsToCreate.Add(new Filings
                    {
                        StatuteOrAct = addFilingsDTO.StatuteOrAct,
                        FormChallan = addFilingsDTO.FormChallan,
                        Particulars = addFilingsDTO.Particulars,
                        DueDate = startDate.AddMonths(i * 6), // Set due date to each half-year
                        DepName = addFilingsDTO.DepName,
                        CreatedById = addFilingsDTO.CreatedById,
                        CreatedDate = DateTime.UtcNow,
                        Status = "Open",
                        DocIsUploaded = false,
                        IsRecurring = isRecurring
                    });
                }
            }
            else if (addFilingsDTO.Recurrence == "1 year")
            {

                isRecurring = true;
                var startDate = addFilingsDTO.DueDate;

                for (int i = 0; i < 3; i++)
                {
                    filingsToCreate.Add(new Filings
                    {
                        StatuteOrAct = addFilingsDTO.StatuteOrAct,
                        FormChallan = addFilingsDTO.FormChallan,
                        Particulars = addFilingsDTO.Particulars,
                        DueDate = startDate.AddYears(1), // Set due date to each year
                        DepName = addFilingsDTO.DepName,
                        CreatedById = addFilingsDTO.CreatedById,
                        CreatedDate = DateTime.UtcNow,
                        Status = "Open",
                        DocIsUploaded = false,
                        IsRecurring = isRecurring
                    });
                }
            }
            else
            {
                throw new ArgumentException("Invalid Recurrence value", nameof(addFilingsDTO.Recurrence));
            }

            // Add filings to the context
            try
            {
                _context.Filings.AddRange(filingsToCreate);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            foreach (var assignedTo in addFilingsDTO.AssignedToList)
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmpName == assignedTo.EmpName);
                if (employee != null)
                {
                    // Create and add UserFiling mappings
                    foreach (var filing in filingsToCreate)
                    {
                        var userFiling = new UserFilings
                        {
                            FilingId = filing.Id, // FilingId will be set after saving filings
                            EmployeeId = employee.EmployeeId
                        };
                        _context.UserFilings.Add(userFiling);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<DepartmentEmployeeDTO>> GetDepartmentEmployeeDTOAsync()
        {
            var departmentWithEmployees = await _context.Departments
                .Select(d => new DepartmentEmployeeDTO
                {
                    Id = d.Id,
                    DepName = d.DepName,
                    Employees = _context.Employees
                        .Where(e => e.DepId == d.Id && e.IsEnabled == true)
                        .Select(e => new Employee
                        {
                            EmployeeId = e.EmployeeId,
                            EmpName = e.EmpName
                        })
                        .ToList()
                })
                .ToListAsync();

            return departmentWithEmployees;
        }

        public async Task<string> ReviewFilingAsync(int filingId, ReviewDTO reviewDTO)
        {

            // Retrieve the updated filing
            var filing = await _context.Set<Filings>()
                .Where(f => f.Id == filingId)
                .FirstOrDefaultAsync();

            if (filing == null)
            {
                throw new Exception("Filing not found");
            }

            // Retrieve employees associated with the filing
            var employees = await _context.Set<UserFilings>()
                .Where(uf => uf.FilingId == filingId)
                .Select(uf => uf.EmployeeId)
                .ToListAsync();

            foreach (var employee in employees)
            {
                var notification = new Notification
                {
                    EmpId = employee,
                    NotificationBody = $"You have a review for the filing {filing.Particulars} which was due on {filing.DueDate.AddDays(1).ToString("dd-MM-yyyy")}",
                    IsRead = false,
                    NotificationType = "Review Filing"
                };

                // Save notification
                await _notificationRepository.AddNotificationAsync(notification);
            }

            // Commit changes to the notifications
            await _notificationRepository.SaveChangesAsync();

            return await _filingsRepository.ReviewFilingAsync(filingId, reviewDTO);
        }

    }
}
