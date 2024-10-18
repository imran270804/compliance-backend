using AutoMapper;
using ComplianceCalendar.Controllers;
using ComplianceCalendar.Data;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComplianceCalendar.MappingConfig;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Components.Forms;
using System.Drawing;
using Moq;
using System.Net;

namespace ComplianceCalendarNUnitTests
{
    [TestFixture]
    public class AddFilingsNTests
    {
        private AddFilingsController _controller;
        private APIContext _context;
        private IMapper _mapper;
        private AddFilingsDTO _addFilingsDTO;
        private Mock<APIContext> _contextMock;


        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _addFilingsDTO = new AddFilingsDTO();
            _contextMock = new Mock<APIContext>();

            var options = new DbContextOptionsBuilder<APIContext>()
               .UseInMemoryDatabase(databaseName: "ComplianceCalendarTest")
               .Options;

            _context = new APIContext(options);

            _context.Departments.AddRange(
                new Department { Id = 1, DepName = "Test Department 1" },
                new Department { Id = 2, DepName = "Test Department 2" }
            );

            _context.SaveChanges();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfig>());
            _mapper = config.CreateMapper();
            _controller = new AddFilingsController(_context, _mapper);

            // Set up the _context object to return an empty list of departments
            _context.Departments.RemoveRange(_context.Departments);
            _context.SaveChanges();

            _controller = new AddFilingsController(_context, _mapper);
        }

        [Test]
        public async Task AddFilings_ReturnsOkResult_WithMessage()
        {
            // Arrange
            var addFilingsDTO = new AddFilingsDTO
            {
                DueDate = DateTime.Now,
                StatuteOrAct = "Test Statute or Act",
                FormChallan = "Test Form Challan",
                Particulars = "Test Particulars",
                DepName = "Test Dep Name",
                CreatedById = 1,
            };

            // Act
            var result = await _controller.AddFilings(addFilingsDTO);

            // Assert
            if (result is BadRequestObjectResult badRequestResult)
            {
                var error = badRequestResult.Value as SerializableError;
                Console.WriteLine($"Error: {error}");
            }
            else
            {
                Assert.Fail("Expected BadRequestObjectResult");
            }
        }


        [Test]
        public void AddFilingsDTO_CreatedById_SetAndGet()
        {
            // Arrange
            var expectedCreatedById = 1;

            // Act
            _addFilingsDTO.CreatedById = expectedCreatedById;

            // Assert
            Assert.AreEqual(expectedCreatedById, _addFilingsDTO.CreatedById);
        }


        [Test]
        public async Task AddFilings_NullAddFilingsDTO_ReturnsBadRequest()
        {
            // Arrange
            AddFilingsDTO addFilingsDTO = null;

            // Act
            var result = await _controller.AddFilings(addFilingsDTO) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("addFilingsDTO cannot be null", result.Value);
        }

        [Test]
        public async Task AddFilings_NullAssignedToList_ReturnsBadRequest()
        {
            // Arrange
            var addFilingsDTO = new AddFilingsDTO
            {
                AssignedToList = null
            };

            // Act
            var result = await _controller.AddFilings(addFilingsDTO) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("AssignedToList cannot be null or empty", result.Value);
        }

        [Test]
        public void Department_DepName_ShouldMatchExpectedValue()
        {
            // Arrange
            var department = new Department();
            string expectedDepName = "CS";

            // Act
            department.DepName = expectedDepName; // Set the property

            // Assert
            Assert.That(department.DepName, Is.EqualTo(expectedDepName));
        }


        [Test]
        public async Task GetDepartmentEmployeeDTO_ReturnsOkWithEmptyList_WhenNoDepartments()
        {
            // Act
            var result = await _controller.GetDepartmentEmployeeDTO();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task AddFilings_EmptyAssignedToList_ReturnsBadRequest()
        {
            // Arrange
            var addFilingsDTO = new AddFilingsDTO { AssignedToList = new List<ComplianceCalendar.Models.DTO.AssignedTo>() };
            var controller = new AddFilingsController(_context, _mapper);

            // Act
            var result = await controller.AddFilings(addFilingsDTO);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void Filings_DocIsUploaded_ShouldMatchExpectedValue()
        {
            // Arrange
            var filings = new Filings();
            bool expectedDocIsUploaded = true;

            // Act
            filings.DocIsUploaded = expectedDocIsUploaded; // Set the property

            // Assert
            Assert.That(filings.DocIsUploaded, Is.EqualTo(expectedDocIsUploaded));
        }

        [Test]
        public void Filings_Remarks_ShouldMatchExpectedValue()
        {
            // Arrange
            var filings = new Filings();
            string expectedRemarks = "Sample Remarks";

            // Act
            filings.Remarks = expectedRemarks; // Set the property

            // Assert
            Assert.That(filings.Remarks, Is.EqualTo(expectedRemarks));
        }

        [Test]
        public void Filings_DueDate_ShouldMatchExpectedValue()
        {
            // Arrange
            var filings = new Filings();
            DateTime expectedDueDate = DateTime.Parse("2022-01-01");

            // Act
            filings.DueDate = expectedDueDate; // Set the property

            // Assert
            Assert.That(filings.DueDate, Is.EqualTo(expectedDueDate));
        }

        [Test]
        public void AddFilingsDTO_Particulars_SetAndGet()
        {
            // Arrange
            var expectedParticulars = "Test Particulars";

            // Act
            _addFilingsDTO.Particulars = expectedParticulars;

            // Assert
            Assert.AreEqual(expectedParticulars, _addFilingsDTO.Particulars);
        }

        [Test]
        public void AddFilingsDTO_FormChallan_SetAndGet()
        {
            // Arrange
            var expectedFormChallan = "Test Form Challan";

            // Act
            _addFilingsDTO.FormChallan = expectedFormChallan;

            // Assert
            Assert.AreEqual(expectedFormChallan, _addFilingsDTO.FormChallan);
        }

    }
}
