using ComplianceCalendar.Controllers;
using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplianceCalendar.Tests
{
    public class ManageUsersControllerTests
    {
        private Mock<IUsersRepository> _repositoryMock;
        private ManageUsersController _controller;
        private ManageUsersController _userController;
        private IUsersRepository _repository;
        private APIContext _context;
        private ManageUserContext _manageUserContext;
        private Mock<ManageUserContext> _manageUserContextMock;


        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IUsersRepository>();
            _manageUserContextMock = new Mock<ManageUserContext>();

        }


        [Test]
        public void ActiveDirectoryUserDto_EmpName_SetAndGet()
        {
            // Arrange
            var user = new ActiveDirectoryUserDto();
            var expectedEmpName = "Hari";

            // Act
            user.EmpName = expectedEmpName;

            // Assert
            Assert.AreEqual(expectedEmpName, user.EmpName);
        }

        [Test]
        public void ActiveDirectoryUserDto_Email_SetAndGet()
        {
            // Arrange
            var user = new ActiveDirectoryUserDto();
            var expectedEmail = "hari@example.com";

            // Act
            user.Email = expectedEmail;

            // Assert
            Assert.AreEqual(expectedEmail, user.Email);
        }
        [Test]
        public void ActiveDirectoryUserDto_DepartmentName_SetAndGet()
        {
            // Arrange
            var user = new ActiveDirectoryUserDto();
            var expectedDepartmentName = "CS";

            // Act
            user.DepartmentName = expectedDepartmentName;

            // Assert
            Assert.AreEqual(expectedDepartmentName, user.DepartmentName);
        }


    }
}