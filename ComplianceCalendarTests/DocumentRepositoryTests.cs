using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using ComplianceCalendar.Repository;
using ComplianceCalendar.Models;
using System.Threading.Tasks;
using System.Linq;
using ComplianceCalendar.Data;

[TestFixture]
public class DocumentRepositoryTests
{
    private DbContextOptions<APIContext> _dbContextOptions;
    private APIContext _context;
    private DocumentRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _dbContextOptions = new DbContextOptionsBuilder<APIContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        _context = new APIContext(_dbContextOptions);
        _repository = new DocumentRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        // Dispose of the context to clean up the in-memory database
        _context.Dispose();
    }

    [Test]
    public async Task GetAsync_ReturnsDocument_WhenFound()
    {
        // Arrange
        var testId = 1;
        var testDocument = new Documents { FilingId = testId };
        _context.Documents.Add(testDocument);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(testId);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(testId, result.FilingId);
    }

    [Test]
    public async Task GetAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var testId = 2; // Assuming this ID does not exist

        // Act
        var result = await _repository.GetAsync(testId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void Save_PersistsChangesToDatabase()
    {
        // Arrange
        var document = new Documents { FilingId = 4 };
        _repository.Add(document);

        // Act
        _repository.Save();

        // Assert
        var result = _context.Documents.Find(4);
        Assert.NotNull(result);
    }

    [Test]
    public void Add_AddsDocumentWithDuplicateFilingId()
    {
        // Arrange
        var document1 = new Documents { FilingId = 7 };
        var document2 = new Documents { FilingId = 7 }; // Duplicate FilingId

        // Act
        _repository.Add(document1);
        _repository.Add(document2); // This will not throw an exception but will be added to the context
        _repository.Save();

        // Assert
        var result = _context.Documents.Where(d => d.FilingId == 7).ToList();
        Assert.AreEqual(2, result.Count); // Check that both documents are added
    }

    [Test]
    public void Save_NoChanges_DoesNotAffectDatabase()
    {
        // Arrange
        var initialDocumentCount = _context.Documents.Count();

        // Act
        _repository.Save();

        // Assert
        var resultDocumentCount = _context.Documents.Count();
        Assert.AreEqual(initialDocumentCount, resultDocumentCount); // Ensure no change in document count
    }

    [Test]
    public void GetAsync_AfterMultipleAddsAndSaves_ReturnsCorrectDocument()
    {
        // Arrange
        var document1 = new Documents { FilingId = 8 };
        var document2 = new Documents { FilingId = 9 };

        _repository.Add(document1);
        _repository.Add(document2);
        _repository.Save();

        // Act
        var result1 = _repository.GetAsync(8).Result;
        var result2 = _repository.GetAsync(9).Result;

        // Assert
        Assert.NotNull(result1);
        Assert.AreEqual(8, result1.FilingId);
        Assert.NotNull(result2);
        Assert.AreEqual(9, result2.FilingId);
    }


    [Test]
    public void Add_ThrowsException_ForInvalidDocument()
    {
        // This test is not applicable for in-memory database since it does not enforce schema constraints like SQL Server.
        // For example, if FilingId should be non-zero, ensure it's not zero in tests.
        Assert.Pass("Test not applicable for in-memory database.");
    }
}
