using ComplianceCalendar.Controllers;
using ComplianceCalendar.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ComplianceCalendar.Models.DTO;
using System.ComponentModel.DataAnnotations;
using ComplianceCalendar.Models;
using Moq;
using NUnit.Framework.Internal;

namespace ComplianceCalendarNUnitTests
{
    [TestFixture]
    public class ExcelUploadNTests
    {
        private UploadFilingsController _controller;
        private APIContext _context;
        private ILogger<UploadFilingsController> _logger;
        private ExcelUploadDTO _excelUploadDTO;
        private Mock<APIContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _excelUploadDTO = new ExcelUploadDTO();
            _contextMock = new Mock<APIContext>(new object[] { });


            // Initialize the context with appropriate options for testing
            var options = new DbContextOptionsBuilder<APIContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new APIContext(options);
            _logger = new Logger<UploadFilingsController>(new LoggerFactory());
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void ExcelUploadDTO_TaskOwnerNames_SetAndGet()
        {
            // Arrange
            var expectedTaskOwnerNames = new List<string> { "Task Owner 1", "Task Owner 2" };

            // Act
            _excelUploadDTO.TaskOwnerNames = expectedTaskOwnerNames;

            // Assert
            Assert.AreEqual(expectedTaskOwnerNames, _excelUploadDTO.TaskOwnerNames);
        }

        [Test]
        public void ExcelUploadDTO_CreatedById_SetAndGet()
        {
            // Arrange
            var expectedCreatedById = 1;

            // Act
            _excelUploadDTO.CreatedById = expectedCreatedById;

            // Assert
            Assert.AreEqual(expectedCreatedById, _excelUploadDTO.CreatedById);
        }

        [Test]
        public void ExcelUploadDTO_Remarks_SetAndGet()
        {
            // Arrange
            var expectedRemarks = "Test Remarks";

            // Act
            _excelUploadDTO.Remarks = expectedRemarks;

            // Assert
            Assert.AreEqual(expectedRemarks, _excelUploadDTO.Remarks);
        }

        [Test]
        public void ExcelUploadDTO_DocIsUploaded_SetAndGet()
        {
            // Arrange
            var expectedDocIsUploaded = true;

            // Act
            _excelUploadDTO.DocIsUploaded = expectedDocIsUploaded;

            // Assert
            Assert.AreEqual(expectedDocIsUploaded, _excelUploadDTO.DocIsUploaded);
        }

        [Test]
        public void ExcelUploadDTO_DepName_SetAndGet()
        {
            // Arrange
            var expectedDepName = "F&A";

            // Act
            _excelUploadDTO.DepName = expectedDepName;

            // Assert
            Assert.AreEqual(expectedDepName, _excelUploadDTO.DepName);
        }


        [Test]
        public void ExcelUploadDTO_Status_SetAndGet()
        {
            // Arrange
            var expectedStatus = "Test Status";

            // Act
            _excelUploadDTO.Status = expectedStatus;

            // Assert
            Assert.AreEqual(expectedStatus, _excelUploadDTO.Status);
        }


        [Test]
        public void ExcelUploadDTO_FormChallan_SetAndGet()
        {
            // Arrange
            var expectedFormChallan = "Test Form Challan";

            // Act
            _excelUploadDTO.FormChallan = expectedFormChallan;

            // Assert
            Assert.AreEqual(expectedFormChallan, _excelUploadDTO.FormChallan);
        }

        [Test]
        public void ExcelUploadDTO_Particulars_SetAndGet()
        {
            // Arrange
            var expectedParticulars = "Test Particulars";

            // Act
            _excelUploadDTO.Particulars = expectedParticulars;

            // Assert
            Assert.AreEqual(expectedParticulars, _excelUploadDTO.Particulars);
        }

        [Test]
        public void ExcelUploadDTO_StatuteOrAct_SetAndGet()
        {
            // Arrange
            var expectedStatuteOrAct = "Test Statute or Act";

            // Act
            _excelUploadDTO.StatuteOrAct = expectedStatuteOrAct;

            // Assert
            Assert.AreEqual(expectedStatuteOrAct, _excelUploadDTO.StatuteOrAct);
        }

        [Test]
        public void ExcelUploadDTO_DueDate_SetAndGet()
        {
            // Arrange
            var expectedDueDate = DateTime.Now;

            // Act
            _excelUploadDTO.DueDate = expectedDueDate;

            // Assert
            Assert.AreEqual(expectedDueDate, _excelUploadDTO.DueDate);
        }

        [Test]
        public void ExcelUploadDTO_SetProperties_SetsPropertiesCorrectly()
        {
            // Arrange
            var dto = new ExcelUploadDTO();

            // Act
            dto.DueDate = new DateTime(2022, 1, 1);
            dto.StatuteOrAct = "StatuteOrAct";
            dto.FormChallan = "FormChallan";
            dto.Particulars = "Particulars";
            dto.Status = "Status";
            dto.DepName = "DepName";
            dto.DocIsUploaded = true;
            dto.Remarks = "Remarks";
            dto.CreatedById = 1;
            dto.CreatedDate = new DateTime(2022, 1, 1);
            dto.TaskOwnerNames = new List<string> { "TaskOwner1", "TaskOwner2" };

            // Assert
            Assert.AreEqual(new DateTime(2022, 1, 1), dto.DueDate);
            Assert.AreEqual("StatuteOrAct", dto.StatuteOrAct);
            Assert.AreEqual("FormChallan", dto.FormChallan);
            Assert.AreEqual("Particulars", dto.Particulars);
            Assert.AreEqual("Status", dto.Status);
            Assert.AreEqual("DepName", dto.DepName);
            Assert.IsTrue(dto.DocIsUploaded);
            Assert.AreEqual("Remarks", dto.Remarks);
            Assert.AreEqual(1, dto.CreatedById);
            Assert.AreEqual(new DateTime(2022, 1, 1), dto.CreatedDate);
            Assert.IsNotNull(dto.TaskOwnerNames);
            Assert.AreEqual(2, dto.TaskOwnerNames.Count);
            Assert.AreEqual("TaskOwner1", dto.TaskOwnerNames[0]);
            Assert.AreEqual("TaskOwner2", dto.TaskOwnerNames[1]);
        }


        [Test]
        public async Task ImportFilings_ValidFile_ReturnsCorrectFormChallan()
        {
            // Arrange
            using (var stream = new MemoryStream())
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells.LoadFromText("Due Date,Statute Or Act,Status,Form/Challan,Particulars,Remarks,Department,Task Owner\n2022-01-01,Test Statute,Open,Test Form,Test Particulars,Test Remarks,Test Department,Test Task Owner");
                    package.Save();
                }

                stream.Position = 0;

                var file = new FormFile(stream, 0, stream.Length, "file", "file.xlsx");

                // Act
                var results = new List<ExcelUploadDTO>
                    {
                        new ExcelUploadDTO
                        {
                            DueDate = DateTime.Parse("2022-01-01"),
                            StatuteOrAct = "Test Statute",
                            Status = "Open",
                            FormChallan = "Test Form",
                            Particulars = "Test Particulars",
                            Remarks = "Test Remarks",
                        }
                    };
                // Assert
                Assert.AreEqual("Test Form", results[0].FormChallan);
            }
        }

        [Test]
        public async Task ImportFilings_ValidFile_VerifiesAllProperties()
        {
            // Arrange
            var fileContent = Encoding.UTF8.GetBytes("Due Date,Statute Or Act,Status,Form/Challan,Particulars,Remarks,Department,Task Owner\n2022-01-01,Test Statute,Open,Test Form,Test Particulars,Test Remarks,Test Department,Test Task Owner");
            var file = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, "file", "file.xlsx");

            // Act
            var results = new List<ExcelUploadDTO>
    {
                new ExcelUploadDTO
                {
                    DueDate = new DateTime(2022, 1, 1),
                    StatuteOrAct = "Test Statute",
                    Status = "Open",
                    FormChallan = "Test Form",
                    Particulars = "Test Particulars",
                    Remarks = "Test Remarks",
                    DepName = "Test Department",
                    CreatedById = 1,
                    CreatedDate = new DateTime(2022, 1, 1),
                    TaskOwnerNames = new List<string> { "Test Task Owner" }
                }
            };
            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(new DateTime(2022, 1, 1), results[0].DueDate);
            Assert.AreEqual("Test Statute", results[0].StatuteOrAct);
            Assert.AreEqual("Open", results[0].Status);
            Assert.AreEqual("Test Form", results[0].FormChallan);
            Assert.AreEqual("Test Particulars", results[0].Particulars);
            Assert.AreEqual("Test Remarks", results[0].Remarks);
            Assert.AreEqual("Test Department", results[0].DepName);
            Assert.AreEqual(1, results[0].CreatedById);
            Assert.AreEqual(new DateTime(2022, 1, 1), results[0].CreatedDate);
            Assert.AreEqual("Test Task Owner", results[0].TaskOwnerNames[0]);
        }

    }
}