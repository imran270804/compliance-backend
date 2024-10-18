using ComplianceCalendar.models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ComplianceCalendar.Endpoints
{
    public static class FilingEndpoints
    {
        public static void MapFilingEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/Filings/UpdateFilingStatus/{id}", async (int id, UpdateFilingStatusDTO updateDTO, IFilingsRepository repo) =>
            {
                if (updateDTO == null)
                {
                    return Results.BadRequest("Invalid data");
                }

                var updateSuccess = await repo.UpdateFilingStatusAsync(id, updateDTO);
                var response = new APIResponse
                {
                    isSuccess = true,
                    Result = updateDTO,
                    StatusCode = HttpStatusCode.OK
                };
                return Results.Ok(response);
            });
        }
    }
}