using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComplianceCalendar.Tests
{
    public class NotificationRepositoryTests
    {
        private NotificationRepository _repository;
        private DbContextOptions<APIContext> _dbContextOptions;

        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            using (var context = new APIContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            var dbContext = new APIContext(_dbContextOptions);
            _repository = new NotificationRepository(dbContext);
        }

        [Test]
        public async Task AddNotification_ShouldAddNotification_WhenValidNotificationIsProvided()
        {
            // Arrange
            var notification = new Notification { Id = 1, EmpId = 1, NotificationBody = "Test Body", NotificationType = "Info", IsRead = false };

            // Act
            await _repository.AddNotificationAsync(notification);
            await _repository.SaveChangesAsync();

            // Assert
            using (var context = new APIContext(_dbContextOptions))
            {
                var savedNotification = await context.Notifications.FindAsync(1);
                Assert.IsNotNull(savedNotification);
                Assert.AreEqual("Test Body", savedNotification.NotificationBody);
            }
        }

        [Test]
        public async Task GetUnreadNotificationsByEmployeeId_ReturnsCorrectNotificationCount_AndNotificationCount_WhenEmployeeIDHasUnreadNotifications()
        {
            // Arrange
            var notification1 = new Notification { Id = 1, EmpId = 1, NotificationBody = "Body 1", NotificationType = "Type 1", IsRead = false };
            var notification2 = new Notification { Id = 2, EmpId = 1, NotificationBody = "Body 2", NotificationType = "Type 2", IsRead = false };
            using (var context = new APIContext(_dbContextOptions))
            {
                context.Notifications.AddRange(notification1, notification2);
                context.SaveChanges();
            }

            // Act
            var result = await _repository.GetUnreadNotificationsByEmpIdAsync(1);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(n => n.NotificationBody == "Body 1"));
        }


        [Test]
        public async Task GetNotificationById_ShouldReturnNotification_WhenIdExists()
        {
            // Arrange
            var notification = new Notification { Id = 1, EmpId = 1, NotificationBody = "Test Body", NotificationType = "Info", IsRead = false };
            using (var context = new APIContext(_dbContextOptions))
            {
                context.Notifications.Add(notification);
                context.SaveChanges();
            }

            // Act
            var result = await _repository.GetNotificationByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Body", result.NotificationBody);
        }

        [Test]
        public async Task GetNotificationById_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Act
            var result = await _repository.GetNotificationByIdAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task MarkAsRead_ShouldUpdateNotificationAsRead_WhenValidNotificationIsProvided()
        {
            // Arrange
            var notification = new Notification { Id = 1, EmpId = 1, NotificationBody = "Test Body", NotificationType = "Info", IsRead = false };
            using (var context = new APIContext(_dbContextOptions))
            {
                context.Notifications.Add(notification);
                context.SaveChanges();
            }

            // Act
            await _repository.MarkAsReadAsync(notification);
            using (var context = new APIContext(_dbContextOptions))
            {
                var updatedNotification = await context.Notifications.FindAsync(1);

                // Assert
                Assert.IsTrue(updatedNotification.IsRead);
            }
        }

        [Test]
        public async Task AddNotification_ShouldPersistNotification_WhenMultipleNotificationsAreAdded()
        {
            // Arrange
            var notification1 = new Notification { Id = 1, EmpId = 1, NotificationBody = "Body 1", NotificationType = "Type 1", IsRead = false };
            var notification2 = new Notification { Id = 2, EmpId = 2, NotificationBody = "Body 2", NotificationType = "Type 2", IsRead = false };

            // Act
            await _repository.AddNotificationAsync(notification1);
            await _repository.AddNotificationAsync(notification2);
            await _repository.SaveChangesAsync();

            // Assert
            using (var context = new APIContext(_dbContextOptions))
            {
                Assert.AreEqual(2, context.Notifications.Count());
                Assert.IsNotNull(await context.Notifications.FindAsync(1));
                Assert.IsNotNull(await context.Notifications.FindAsync(2));
            }
        }

        [Test]
        public async Task GetUnreadNotificationsByEmpId_ShouldReturnEmptyList_WhenNoUnreadNotifications()
        {
            // Act
            var result = await _repository.GetUnreadNotificationsByEmpIdAsync(1);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task MarkAsRead_ShouldHandleConcurrentUpdates_WhenMultipleUpdatesAreMade()
        {
            // Arrange
            var notification = new Notification { Id = 1, EmpId = 1, NotificationBody = "Test Body", NotificationType = "Info", IsRead = false };
            using (var context = new APIContext(_dbContextOptions))
            {
                context.Notifications.Add(notification);
                context.SaveChanges();
            }

            // Act
            var task1 = Task.Run(async () =>
            {
                var repo = new NotificationRepository(new APIContext(_dbContextOptions));
                await repo.MarkAsReadAsync(notification);
            });

            var task2 = Task.Run(async () =>
            {
                var repo = new NotificationRepository(new APIContext(_dbContextOptions));
                await repo.MarkAsReadAsync(notification);
            });

            await Task.WhenAll(task1, task2);

            // Assert
            using (var context = new APIContext(_dbContextOptions))
            {
                var updatedNotification = await context.Notifications.FindAsync(1);
                Assert.IsTrue(updatedNotification.IsRead);
            }
        }

        [Test]
        public async Task AddNotification_ShouldNotDuplicate_WhenAddingNotificationWithSameId()
        {
            // Arrange
            var notification1 = new Notification { Id = 1, EmpId = 1, NotificationBody = "Body 1", NotificationType = "Type 1", IsRead = false };
            var notification2 = new Notification { Id = 1, EmpId = 2, NotificationBody = "Body 2", NotificationType = "Type 2", IsRead = false };

            // Act
            await _repository.AddNotificationAsync(notification1);
            await _repository.SaveChangesAsync();

            // Adding duplicate ID (simulating conflict)
            var exceptionThrown = false;
            try
            {
                await _repository.AddNotificationAsync(notification2);
                await _repository.SaveChangesAsync();
            }
            catch
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.IsTrue(exceptionThrown);

            using (var context = new APIContext(_dbContextOptions))
            {
                Assert.AreEqual(1, context.Notifications.Count());
                var savedNotification = await context.Notifications.FindAsync(1);
                Assert.AreEqual("Body 1", savedNotification.NotificationBody);
            }
        }
    }
}
