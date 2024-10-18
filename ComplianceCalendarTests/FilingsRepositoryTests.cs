using NUnit.Framework;
using Moq;
using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repository;
using ComplianceCalendar.Repository.IRepository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ComplianceCalendar.Tests
{
    [TestFixture]
    public class FilingsRepositoryTests
    {
        private APIContext _context;
        private Mock<IMapper> _mapperMock;
        private FilingsRepository _repository;
        private Mock<INotificationRepository> _notificationRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            // Set up InMemory database context
            var options = new DbContextOptionsBuilder<APIContext>()
                .UseInMemoryDatabase(databaseName: "ComplianceCalendarTest" + Guid.NewGuid()) // Ensure unique database name
                .Options;

            _context = new APIContext(options);
            _mapperMock = new Mock<IMapper>();
            _notificationRepositoryMock = new Mock<INotificationRepository>();

            _repository = new FilingsRepository(_context, _mapperMock.Object, _notificationRepositoryMock.Object);

            // Seed data
            var filings = new Filings
            {
                Id = 1,
                DueDate = DateTime.Now,
                StatuteOrAct = "Act 1",
                FormChallan = "Form 1",
                Particulars = "Particulars 1",
                Status = "Status 1",
                DepName = "Dep 1",
                DocIsUploaded = true,
                Remarks = "Remarks 1"
            };

            _context.Filings.Add(filings);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAdminFilingsAsync_ThrowsException_WhenYearIsLessThan2000()
        {
            // Arrange
            var employeeId = 1;
            var year = 1800;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _repository.GetAdminFilingsAsync(employeeId, year));
        }

        [Test]
        public async Task GetAdminFilingsAsync_ReturnsIEnumerableOfObjects()
        {
            // Arrange
            int employeeId = 1;
            int year = 2022;

            // Act
            var result = await _repository.GetAdminFilingsAsync(employeeId, year);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<object>>(result);
        }

        [Test]
        public async Task GetUserFilingsAsync_ReturnsIEnumerableOfObjects()
        {
            // Arrange
            int employeeId = 1;
            int year = 2022;

            // Act
            var result = await _repository.GetUserFilingsAsync(employeeId, year);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<object>>(result);
        }

        [Test]
        public async Task GetFilingByIdAsync_ReturnsFilings()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _repository.GetFilingByIdAsync(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Filings>(result);
        }

        [Test]
        public void Filings_MapsProperties()
        {
            // Arrange
            var filings = new Filings
            {
                Id = 1,
                StatuteOrAct = "Act 1",
                FormChallan = "Form 1",
                Particulars = "Particulars 1",
                Status = "Status 1",
                DepName = "Dep 1",
                DocIsUploaded = true,
                Remarks = "Remarks 1"
            };

            // Act & Assert
            Assert.AreEqual(1, filings.Id);
            Assert.AreEqual("Act 1", filings.StatuteOrAct);
            Assert.AreEqual("Form 1", filings.FormChallan);
            Assert.AreEqual("Particulars 1", filings.Particulars);
            Assert.AreEqual("Status 1", filings.Status);
            Assert.AreEqual("Dep 1", filings.DepName);
            Assert.IsTrue(filings.DocIsUploaded);
            Assert.AreEqual("Remarks 1", filings.Remarks);
        }

   

        [Test]
        public async Task FilingExistsAsync_ReturnsTrue()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _repository.FilingExistsAsync(id);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task AddUserFilingsAsync_ReturnsTrue()
        {
            // Arrange
            var userFilings = new List<UserFilings> { new UserFilings { Id = 1, EmployeeId = 1, FilingId = 1 } };

            // Act
            var result = await _repository.AddUserFilingsAsync(userFilings);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task GetDepartmentEmployeeDTOsAsync_ReturnsIEnumerableOfDepartmentEmployeeDTO()
        {
            // Act
            var result = await _repository.GetDepartmentEmployeeDTOsAsync(CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<DepartmentEmployeeDTO>>(result);
        }

        [Test]
        public async Task ReviewFilingAsync_ReturnsSuccessMessage()
        {
            // Arrange
            int filingId = 1;
            var reviewDTO = new ReviewDTO { Review = "This is a review." };

            // Mock the behavior of notification repository
            _notificationRepositoryMock.Setup(nr => nr.AddNotificationAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);
            _notificationRepositoryMock.Setup(nr => nr.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _repository.ReviewFilingAsync(filingId, reviewDTO);

            // Assert
            Assert.AreEqual("Review added successfully.", result);

            // Verify no notification added for review only
            _notificationRepositoryMock.Verify(nr => nr.AddNotificationAsync(It.IsAny<Notification>()), Times.Never);
        }
    }
}
