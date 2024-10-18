using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Collections.Generic;
using ComplianceCalendar.Data;

public static class AuthenticationEndpoint
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
         app.MapGet("/getUserRole/{token}", (string token, IServiceProvider services) =>
         {
             Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();

             if (token != null)
             {
                 var jwtStream = token;
                 var handler = new JwtSecurityTokenHandler();

                 var jsonToken = handler.ReadToken(jwtStream);
                 var tokenS = jsonToken as JwtSecurityToken;
                 if (tokenS != null)
                 {
                     var claims = tokenS.Claims;
                     var headers = tokenS.Header;
                     var payloads = tokenS.Payload;

                     // UPN From Jwt token
                     var upn = claims.FirstOrDefault(c => c.Type == "upn").Value;
                     // Fetch email id from employee database.

                     using (var scope = services.CreateScope())
                     {
                         var dbContext = scope.ServiceProvider.GetRequiredService<ManageUserContext>();
                         var employee = dbContext.Employee.FirstOrDefault(e => e.Email == upn);

                         if (employee != null)
                         {
                             // Return the employee details such as email, name, role, department
                             var roles = dbContext.Roles.FirstOrDefault(r => r.Id == employee.RoleId);
                             var departments = dbContext.Departments.FirstOrDefault(de => de.Id == employee.DepId);
                             /*results.Add("appName", appName);*/
                             results.Add("EmployeeId", employee.EmployeeId);
                             results.Add("EmployeeName", employee.EmpName);
                             results.Add("employeeEmail", employee.Email);
                             results.Add("department", departments.DepName);
                             results.Add("roleName", roles.Rolename);
                             results.Add("departmentId", employee.DepId);
                             results.Add("isEnabled",employee.IsEnabled);
                         }
                         else
                         {
                             Console.WriteLine($"{upn} is not found in Employees database..");
                         }
                     }
                 }
                 else
                 {
                     Console.WriteLine("Token Cannot converted to JwtSecurityToken");
                 }
             }
             else
             {
                 return Results.NotFound("Token not found");
             }
             return Results.Ok(results);
         })
         .WithName("GetUserRole");

        /*app.MapGet("/getUserRoles", (string token, IServiceProvider services) =>
        {
            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();

            if (!string.IsNullOrEmpty(token))
            {
                var jwtStream = token;
                var handler = new JwtSecurityTokenHandler();

                var jsonToken = handler.ReadToken(jwtStream);
                var tokenS = jsonToken as JwtSecurityToken;
                if (tokenS != null)
                {
                    var claims = tokenS.Claims;
                    var headers = tokenS.Header;
                    var payloads = tokenS.Payload;

                    // UPN From Jwt token
                    var upn = claims.FirstOrDefault(c => c.Type == "upn")?.Value;
                    if (upn != null)
                    {
                        using (var scope = services.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<ManageUserContext>();
                            var employee = dbContext.Employee.FirstOrDefault(e => e.Email == upn);

                            if (employee != null)
                            {
                                // Return the employee details such as email, name, role, department
                                var roles = dbContext.Roles.FirstOrDefault(r => r.Id == employee.RoleId);
                                var departments = dbContext.Departments.FirstOrDefault(de => de.Id == employee.DepId);
                                results.Add("EmployeeId", employee.EmployeeId);
                                results.Add("EmployeeName", employee.EmpName);
                                results.Add("employeeEmail", employee.Email);
                                results.Add("department", departments?.DepName);
                                results.Add("roleName", roles?.Rolename);
                                results.Add("departmentId", employee.DepId);
                                results.Add("isEnabled", employee.IsEnabled);
                            }
                            else
                            {
                                Console.WriteLine($"{upn} is not found in the Employees database.");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Token cannot be converted to JwtSecurityToken.");
                }
            }
            else
            {
                return Results.NotFound("Token not found");
            }
            return Results.Ok(results);
        })
.WithName("GetUserRole");
*/
    }
}
