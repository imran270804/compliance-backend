using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Repository;

namespace ComplianceCalendar.Tests
{
    public class EMailRepositoryTests
    {
        private DbContextOptions<APIContext> _dbContextOptions;
        private EMailRepository _repository;

        [SetUp]
        public void Setup()
        {
            // Set up the in-memory database
            _dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Initialize repository
            var context = new APIContext(_dbContextOptions);
            _repository = new EMailRepository(context);
        }

        [Test]
        public async Task GetUserFilingsByFilingIdAsync_ShouldReturnUserFilings_WhenValidFilingIdIsProvided()
        {
            // Arrange
            var filingId = 1;
            var userFilings = new List<UserFilings>
            {
                new UserFilings { FilingId = filingId, EmployeeId = 1 },
                new UserFilings { FilingId = filingId, EmployeeId = 2 }
            };
            using (var context = new APIContext(_dbContextOptions))
            {
                context.Set<UserFilings>().AddRange(userFilings);
                context.SaveChanges();
            }

            // Act
            var result = await _repository.GetUserFilingsByFilingIdAsync(filingId);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(uf => uf.EmployeeId == 1));
            Assert.IsTrue(result.Any(uf => uf.EmployeeId == 2));
        }

        [Test]
        public async Task GetEmployeesByIdsAsync_ShouldReturnEmployees_WhenValidIdsAreProvided()
        {
            // Arrange
            var employeeIds = new List<int> { 1, 2 };
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, EmpName = "Alice" },
                new Employee { EmployeeId = 2, EmpName = "Bob" }
            };
            using (var context = new APIContext(_dbContextOptions))
            {
                context.Set<Employee>().AddRange(employees);
                context.SaveChanges();
            }

            // Act
            var result = await _repository.GetEmployeesByIdsAsync(employeeIds);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(e => e.EmpName == "Alice"));
            Assert.IsTrue(result.Any(e => e.EmpName == "Bob"));
        }

        [Test]
        public async Task GetFilingsDueSoonAsync_ShouldReturnFilingsWithinDateRangeAndStatus()
        {
            // Arrange
            var startDate = new DateTimeOffset(new DateTime(2024, 7, 1));
            var endDate = new DateTimeOffset(new DateTime(2024, 7, 31));
            var status = "Pending";
            var filings = new List<Filings>
            {
                new Filings { DueDate = new DateTime(2024, 7, 10), Status = status },
                new Filings { DueDate = new DateTime(2024, 6, 30), Status = status },
                new Filings { DueDate = new DateTime(2024, 7, 15), Status = "Closed" }
            };
            using (var context = new APIContext(_dbContextOptions))
            {
                context.Set<Filings>().AddRange(filings);
                context.SaveChanges();
            }

            // Act
            var result = await _repository.GetFilingsDueSoonAsync(startDate, endDate, status);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(new DateTime(2024, 7, 10), result[0].DueDate);
            Assert.AreEqual(status, result[0].Status);
        }

        [Test]
        public async Task GetFilingsDueSoonAsync_ShouldReturnEmptyList_WhenNoFilingsMatchDateRangeAndStatus()
        {
            // Arrange
            var startDate = new DateTimeOffset(new DateTime(2024, 7, 1));
            var endDate = new DateTimeOffset(new DateTime(2024, 7, 31));
            var status = "Pending";
            var filings = new List<Filings>
            {
                new Filings { DueDate = new DateTime(2024, 8, 1), Status = "Pending" },
                new Filings { DueDate = new DateTime(2024, 7, 5), Status = "Closed" }
            };
            using (var context = new APIContext(_dbContextOptions))
            {
                context.Set<Filings>().AddRange(filings);
                context.SaveChanges();
            }

            // Act
            var result = await _repository.GetFilingsDueSoonAsync(startDate, endDate, status);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task GetUserFilingsByFilingIdAsync_ShouldReturnEmptyList_WhenNoUserFilingsExistForFilingId()
        {
            // Arrange
            var filingId = 100; // Use a filing ID that does not exist
            using (var context = new APIContext(_dbContextOptions))
            {
                // Ensure no filings with this ID exist
                context.Set<UserFilings>().RemoveRange(context.Set<UserFilings>());
                context.SaveChanges();
            }

            // Act
            var result = await _repository.GetUserFilingsByFilingIdAsync(filingId);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task GetEmployeesByIdsAsync_ShouldReturnEmptyList_WhenNoEmployeesMatchProvidedIds()
        {
            // Arrange
            var employeeIds = new List<int> { 999, 1000 }; // IDs that do not exist
            using (var context = new APIContext(_dbContextOptions))
            {
                // Ensure no employees with these IDs exist
                context.Set<Employee>().RemoveRange(context.Set<Employee>());
                context.SaveChanges();
            }

            // Act
            var result = await _repository.GetEmployeesByIdsAsync(employeeIds);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

    }
}
