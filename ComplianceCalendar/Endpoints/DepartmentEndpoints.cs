using ComplianceCalendar.Repository.IRepository;
using ComplianceCalendar.Models.DTO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ComplianceCalendar.Endpoints
{
    public static class DepartmentEndpoints
    {
        public static void MapDepartmentEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/departments", async (IDepartmentRepository repo, IMapper mapper, ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("DepartmentEndpoints");
                logger.LogInformation("Get all departments endpoint called");

                var departments = await repo.GetAllDepartmentsAsync();

                if (departments == null || !departments.Any())
                {
                    logger.LogWarning("No departments found");
                    return Results.NotFound("No departments found.");
                }

                var departmentDTOs = mapper.Map<List<GetDepartmentDTO>>(departments);
                logger.LogInformation("Departments retrieved successfully");
                return Results.Ok(departmentDTOs);
            });
        }
    }
}
