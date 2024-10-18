using AutoMapper;
using ComplianceCalendar.models;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repository.IRepository;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ComplianceCalendar.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _docRepo;
        private readonly IMapper _mapper;
        private readonly string _fileStoragePath;

        public DocumentService(IDocumentRepository docRepo, IMapper mapper)
        {
            _docRepo = docRepo;
            _mapper = mapper;
            _fileStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedDocuments");
        }

        public async Task<APIResponse> AddDocumentAsync(AddDocumentDTO addDocumentDTO)
        {
            try
            {
                if (addDocumentDTO.File == null || addDocumentDTO.File.Length == 0)
                {
                    return new APIResponse { isSuccess = false, Result = "No file selected.", StatusCode = HttpStatusCode.BadRequest };
                }

                if (!Directory.Exists(_fileStoragePath))
                {
                    Directory.CreateDirectory(_fileStoragePath);
                }

                string fileName = Path.GetFileName(addDocumentDTO.File.FileName);
                string filePath = Path.Combine(_fileStoragePath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await addDocumentDTO.File.CopyToAsync(stream);
                }

                var document = _mapper.Map<Documents>(addDocumentDTO);
                document.UploadedDate = DateTime.UtcNow;
                document.UploadedBy = 3; // Handle this dynamically if needed
                document.DocumentLink = filePath;

                _docRepo.Add(document);
                _docRepo.Save();

                return new APIResponse { isSuccess = true, Result = document, StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return new APIResponse { isSuccess = false, Result = $"An error occurred while adding the document: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<byte[]> DownloadDocumentAsync(int filingId)
        {
            try
            {
                string documentPath = await GetDocumentPathFromDatabase(filingId);
                return await File.ReadAllBytesAsync(documentPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading document: {ex.Message}");
            }
        }

        public async Task<APIResponse> DeleteDocumentAsync(int documentId)
        {
            try
            {
                // Fetch the document from the database
                var document = await _docRepo.GetAsync(documentId);

                if (document == null)
                {
                    return new APIResponse { isSuccess = false, Result = "Document not found.", StatusCode = HttpStatusCode.NotFound };
                }

                // Delete the physical file
                if (File.Exists(document.DocumentLink))
                {
                    File.Delete(document.DocumentLink);
                }

                // Remove the document from the database
                _docRepo.Delete(documentId);
                _docRepo.Save();

                return new APIResponse { isSuccess = true, Result = "Document deleted successfully.", StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return new APIResponse { isSuccess = false, Result = $"An error occurred while deleting the document: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        private async Task<string> GetDocumentPathFromDatabase(int documentId)
        {
            var document = await _docRepo.GetAsync(documentId);
            var documentDto = _mapper.Map<GetDocLinkDTO>(document);
            return documentDto.DocumentLink;
        }
    }
}
