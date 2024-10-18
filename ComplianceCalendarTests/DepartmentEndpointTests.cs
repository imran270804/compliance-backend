using AutoMapper;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repository.IRepository;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplianceCalendar.Tests
{
    public class DepartmentEndpointsTests
    {
        private Mock<IDepartmentRepository> _departmentRepositoryMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetUp()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Test]
        public async Task GetDepartments_ReturnsOkResultWithDepartments()
        {
            // Arrange
            var departments = new List<Department>
            {
                new Department { Id = 1, DepName = "HR" },
                new Department { Id = 2, DepName = "IT" }
            };
            var departmentDTOs = new List<GetDepartmentDTO>
            {
                new GetDepartmentDTO { Id = 1, DepName = "HR" },
                new GetDepartmentDTO { Id = 2, DepName = "IT" }
            };

            _departmentRepositoryMock.Setup(repo => repo.GetAllDepartmentsAsync()).ReturnsAsync(departments);
            _mapperMock.Setup(m => m.Map<List<GetDepartmentDTO>>(departments)).Returns(departmentDTOs);

            // Act
            var result = await GetDepartmentsAsync(_departmentRepositoryMock.Object, _mapperMock.Object);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("HR", result[0].DepName);
            Assert.AreEqual("IT", result[1].DepName);
        }

        private async Task<List<GetDepartmentDTO>> GetDepartmentsAsync(IDepartmentRepository repo, IMapper mapper)
        {
            var departments = await repo.GetAllDepartmentsAsync();
            return mapper.Map<List<GetDepartmentDTO>>(departments);
        }
    }
}
