using NUnit.Framework;
using ComplianceCalendar.Models;

namespace ComplianceCalendar.Tests
{
    [TestFixture]
    public class UserFilingsTests
    {
        private UserFilings userFilings;

        [SetUp]
        public void SetUp()
        {
            userFilings = new UserFilings();
        }

        [TearDown]
        public void TearDown()
        {
            userFilings = null;
        }

        [Test]
        public void UserFilings_GetEmployeeId_ReturnsValue()
        {
            // Arrange
            var userFilings = new UserFilings();
            int employeeId = 1;
            userFilings.EmployeeId = employeeId;

            // Act and Assert
            Assert.AreEqual(employeeId, userFilings.EmployeeId);
        }

        [Test]
        public void UserFilings_Constructor_DefaultValues()
        {
            // Assert
            Assert.IsNull(userFilings.Filings);
            Assert.IsNull(userFilings.Employee);
            Assert.AreEqual(0, userFilings.Id);
            Assert.AreEqual(0, userFilings.FilingId);
            Assert.AreEqual(0, userFilings.EmployeeId);
        }

        [Test]
        public void UserFilings_SetFilingId_ValidValue()
        {
            // Arrange
            int filingId = 1;

            // Act
            userFilings.FilingId = filingId;

            // Assert
            Assert.AreEqual(filingId, userFilings.FilingId);
        }

        [Test]
        public void UserFilings_SetEmployeeId_ValidValue()
        {
            // Arrange
            int employeeId = 1;

            // Act
            userFilings.EmployeeId = employeeId;

            // Assert
            Assert.AreEqual(employeeId, userFilings.EmployeeId);
        }
    }
}