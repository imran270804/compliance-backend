using ComplianceCalendar.Controllers;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class AdminControllerTests
{
    private Mock<IAdminRepository> _repositoryMock;
    private Mock<ILogger<AdminController>> _loggerMock;
    private AdminController _controller;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IAdminRepository>();
        _loggerMock = new Mock<ILogger<AdminController>>(); // Ensure logger mock is instantiated
        _controller = new AdminController(_repositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GetAdmins_ReturnsOkResultWithAdmins()
    {
        // Arrange
        var admins = new List<AdminDTO>
        {
            new AdminDTO { EmployeeId = 1, EmpName = "Admin1", Email = "admin1@example.com", DepartmentName = "HR" },
            new AdminDTO { EmployeeId = 2, EmpName = "Admin2", Email = "admin2@example.com", DepartmentName = "IT" }
        };

        _repositoryMock.Setup(repo => repo.GetAdminsAsync()).ReturnsAsync(admins);

        // Act
        var result = await _controller.GetAdmins();

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(admins, okResult.Value);
    }

    [Test]
    public async Task GetAdmins_ReturnsNotFoundResultWhenNoAdmins()
    {
        // Arrange
        _repositoryMock.Setup(repo => repo.GetAdminsAsync()).ReturnsAsync(new List<AdminDTO>());

        // Act
        var result = await _controller.GetAdmins();

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.AreEqual("No admins found.", notFoundResult.Value);
    }

    [Test]
    public async Task GetDepartmentNamesByEmployeeId_ReturnsOkResultWithDepartments()
    {
        // Arrange
        var departments = new List<Department>
        {
            new Department { Id = 1, DepName = "HR" },
            new Department { Id = 2, DepName = "IT" }
        };

        _repositoryMock.Setup(repo => repo.GetDepartmentNamesByAdminIdAsync(It.IsAny<int>())).ReturnsAsync(departments);

        // Act
        var result = await _controller.GetDepartmentNamesByEmployeeId(1);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(departments, okResult.Value);
    }

    [Test]
    public async Task DeleteAdmin_ReturnsNoContentResultWhenAdminDeleted()
    {
        // Arrange
        var admin = new AdminDTO { EmployeeId = 1, EmpName = "Admin1", Email = "admin1@example.com", DepartmentName = "HR" };

        _repositoryMock.Setup(repo => repo.DeleteAdminAsync(It.IsAny<int>())).ReturnsAsync(admin);

        // Act
        var result = await _controller.DeleteAdmin(1);

        // Assert
        Assert.IsInstanceOf<NoContentResult>(result);
        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [Test]
    public async Task DeleteAdmin_ReturnsNotFoundResultWhenAdminNotFound()
    {
        // Arrange
        _repositoryMock.Setup(repo => repo.DeleteAdminAsync(It.IsAny<int>())).ReturnsAsync((AdminDTO)null);

        // Act
        var result = await _controller.DeleteAdmin(1);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.AreEqual("Employee not found.", notFoundResult.Value);
    }

    [Test]
    public async Task AddAdmin_ReturnsOkResultWithAdmin()
    {
        // Arrange
        var model = new AddAdminDTO { EmpName = "Admin1", Email = "admin1@example.com", DepartmentName = "HR" };
        var admin = new AdminDTO { EmployeeId = 1, EmpName = "Admin1", Email = "admin1@example.com", DepartmentName = "HR" };

        _repositoryMock.Setup(repo => repo.AddAdminAsync(model)).ReturnsAsync(admin);

        // Act
        var result = await _controller.AddAdmin(model);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(admin, okResult.Value);
    }

    [Test]
    public async Task AddAdmin_ReturnsInternalServerErrorWhenExceptionThrown()
    {
        // Arrange
        var model = new AddAdminDTO { EmpName = "Admin1", Email = "admin1@example.com", DepartmentName = "HR" };

        _repositoryMock.Setup(repo => repo.AddAdminAsync(It.IsAny<AddAdminDTO>())).ThrowsAsync(new Exception("Test Exception"));

        // Act
        var result = await _controller.AddAdmin(model);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result.Result);
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        Assert.IsTrue(objectResult.Value.ToString().Contains("Failed to add admin: Test Exception"));
    }
}
