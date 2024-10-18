using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ComplianceCalendar.Repositories
{
    public class UploadFilingsRepository : IUploadFilingsRepository
    {
        private readonly APIContext _context;
        private readonly ILogger<UploadFilingsRepository> _logger;

        public UploadFilingsRepository(APIContext context, ILogger<UploadFilingsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ProcessUploadAsync(IFormFile file, int createdById, CancellationToken ct)
        {
            var filingsDTOs = ImportFilings(file, createdById);

            // Retrieve all employees into memory
            var employees = await _context.Employees.AsNoTracking().ToListAsync(ct);

            // Convert DTOs to entities and save to database
            foreach (var filingsDTO in filingsDTOs)
            {
                var createdByEmployee = employees.FirstOrDefault(e => e.EmployeeId == filingsDTO.CreatedById);
                if (createdByEmployee == null)
                {
                    _logger.LogWarning($"CreatedById {filingsDTO.CreatedById} not found in Employee table.");
                    continue;
                }

                var filing = new Filings
                {
                    DueDate = filingsDTO.DueDate,
                    StatuteOrAct = filingsDTO.StatuteOrAct,
                    Status = filingsDTO.Status,
                    FormChallan = filingsDTO.FormChallan,
                    Particulars = filingsDTO.Particulars,
                    Remarks = filingsDTO.Remarks,
                    CreatedById = filingsDTO.CreatedById,
                    CreatedDate = DateTime.UtcNow,
                    DepName = filingsDTO.DepName,
                };

                _context.Filings.Add(filing);
                await _context.SaveChangesAsync(); // Save to get the filing ID

                // Process Task Owners
                if (filingsDTO.TaskOwnerNames != null)
                {
                    foreach (var taskOwnerName in filingsDTO.TaskOwnerNames)
                    {
                        var employee = employees.FirstOrDefault(e =>
                            string.Equals(
                                RemoveWhiteSpaces(e.EmpName),
                                RemoveWhiteSpaces(taskOwnerName),
                                StringComparison.OrdinalIgnoreCase
                            )
                        );

                        if (employee != null)
                        {
                            var userFiling = new UserFilings
                            {
                                FilingId = filing.Id,
                                EmployeeId = employee.EmployeeId,
                            };
                            _context.UserFilings.Add(userFiling);
                        }
                        else
                        {
                            _logger.LogWarning($"Employee not found for Task Owner: {taskOwnerName}");
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private List<ExcelUploadDTO> ImportFilings(IFormFile file, int createdById)
        {
            var filings = new List<ExcelUploadDTO>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = file.OpenReadStream())
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        if (reader.Depth == 0) // Skip header row
                        {
                            continue;
                        }

                        var dueDate = reader.IsDBNull(1) ? (DateTime?)null : reader.GetDateTime(1).ToUniversalTime();
                        if (!dueDate.HasValue)
                        {
                            break;
                        }
                        var statuteOrAct = reader.IsDBNull(2) ? null : reader.GetString(2);
                        var status = reader.IsDBNull(3) ? "Open" : reader.GetString(3);
                        var formChallan = reader.IsDBNull(4) ? null : reader.GetString(4);
                        var particulars = reader.IsDBNull(5) ? "" : reader.GetString(5);
                        var remarks = reader.IsDBNull(6) ? "" : reader.GetString(6);
                        var depName = reader.IsDBNull(7) ? null : reader.GetString(7);
                        var taskOwner = reader.IsDBNull(8) ? null : reader.GetString(8)?.Split(',').Select(s => s.Trim()).ToList();

                        var filing = new ExcelUploadDTO
                        {
                            DueDate = dueDate.Value,
                            StatuteOrAct = statuteOrAct,
                            Status = status,
                            FormChallan = formChallan,
                            Particulars = particulars,
                            Remarks = remarks,
                            CreatedById = createdById,
                            CreatedDate = DateTime.UtcNow,
                            DepName = depName,
                            TaskOwnerNames = taskOwner
                        };

                        filings.Add(filing);
                    }
                }
            }

            return filings;
        }

        private static string RemoveWhiteSpaces(string input)
        {
            return new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
    }
}
