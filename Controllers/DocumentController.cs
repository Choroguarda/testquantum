using Microsoft.AspNetCore.Mvc;
using TestQuantumDocs.DTOs;
using TestQuantumDocs.Models;
using TestQuantumDocs.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace TestQuantumDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DocumentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateDocument([FromBody] CreateDocumentRequestDto dto)
        {
            var errors = ValidateDocument(dto);
            if (errors.Any())
            {
                return BadRequest(new { error = errors });
            }

            var document = new Document
            {
                Name = dto.Name,
                Description = dto.Description,
                AuthorFullName = dto.AuthorFullName,
                AuthorEmail = dto.AuthorEmail,
                SerialCode = dto.SerialCode,
                PublicationCode = dto.PublicationCode,
                CreatedAt = DateTime.UtcNow,
                Deleted = false,
                DocumentPageIndex = dto.Indexes.Select(i => new DocumentPageIndex
                {
                    Name = i.Name,
                    Page = i.Page,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            _context.Documents.Add(document);
            _context.SaveChanges();

            return Ok(document);
        }

        private List<string> ValidateDocument(CreateDocumentRequestDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Name es obligatorio.");
            else if (dto.Name.Length > 100)
                errors.Add("Name excede el tamaño máximo de caracteres.");

            if (string.IsNullOrWhiteSpace(dto.AuthorFullName))
                errors.Add("AuthorFullName es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.AuthorEmail) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(dto.AuthorEmail))
                errors.Add("AuthorEmail no es válido.");

            if (string.IsNullOrWhiteSpace(dto.SerialCode) || !System.Text.RegularExpressions.Regex.IsMatch(dto.SerialCode, @"\A\b[0-9a-fA-F]+\b\Z"))
                errors.Add("SerialCode debe estar en formato hexadecimal.");

            if (string.IsNullOrWhiteSpace(dto.PublicationCode) ||
                !(dto.PublicationCode.StartsWith("ISO-") ||
                  dto.PublicationCode.StartsWith("Ley N° ") ||
                  dto.PublicationCode.StartsWith("P-")))
                errors.Add("PublicationCode no es válido.");

            if (dto.Indexes == null || !dto.Indexes.Any())
                errors.Add("Debe haber al menos un índice.");

            return errors;
        }
        private List<string> ValidateDocumentUpdate(UpdateDocumentRequestDto dto)
        {
            var errors = new List<string>();

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name.Length > 100)
                errors.Add("Name excede el tamaño máximo de caracteres.");

            if (!string.IsNullOrWhiteSpace(dto.AuthorEmail) &&
                !new EmailAddressAttribute().IsValid(dto.AuthorEmail))
                errors.Add("AuthorEmail no es válido.");

            if (!string.IsNullOrWhiteSpace(dto.SerialCode) &&
                !Regex.IsMatch(dto.SerialCode, @"\A\b[0-9a-fA-F]+\b\Z"))
                errors.Add("SerialCode debe estar en formato hexadecimal.");

            if (!string.IsNullOrWhiteSpace(dto.PublicationCode) &&
                !(dto.PublicationCode.StartsWith("ISO-") ||
                dto.PublicationCode.StartsWith("Ley N° ") ||
                dto.PublicationCode.StartsWith("P-")))
                errors.Add("PublicationCode no es válido.");

            // Si se envían índices, al menos uno debe existir
            if (dto.Indexes != null && !dto.Indexes.Any())
                errors.Add("Debe haber al menos un índice.");

            return errors;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            var document = await _context.Documents
                .Include(d => d.DocumentPageIndex)
                .FirstOrDefaultAsync(d => d.Id == id && d.DeletedAt == null);

            if (document == null)
                return NotFound(new { message = $"Documento con ID {id} no encontrado." });
    
            var result = new DocumentDetailDto
            {
                Id = document.Id,
                Name = document.Name,
                Description = document.Description,
                AuthorFullName = document.AuthorFullName,
                AuthorEmail = document.AuthorEmail,
                SerialCode = document.SerialCode,
                PublicationCode = document.PublicationCode,
                CreatedAt = document.CreatedAt,
                Indexes = document.DocumentPageIndex.Select(i => new DocumentPageIndexDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Page = i.Page
                }).ToList()
            };

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id && d.DeletedAt == null);
            if (doc == null)
                return NotFound("Documento no encontrado o ya eliminado.");

            doc.DeletedAt = DateTime.UtcNow;
            doc.Deleted = true;
            await _context.SaveChangesAsync();

            return Ok("Documento eliminado correctamente.");
        }

            [HttpPost("search")]
            public async Task<ActionResult<IEnumerable<DocumentSummaryDto>>> SearchDocuments([FromBody] SearchDocumentsRequestDto request)
            {
                const int PageSize = 10;
                int pageNumber = request.Page < 1 ? 1 : request.Page;

                var query = _context.Documents
                    .Where(d => d.DeletedAt == null); // Excluye eliminados

                if (request.Id.HasValue && request.Id > 0)
                {
                    query = query.Where(d => d.Id == request.Id.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.SerialCode))
                {
                    query = query.Where(d => d.SerialCode.ToLower() == request.SerialCode.ToLower());
                }

                if (!string.IsNullOrWhiteSpace(request.PublicationCode))
                {
                    query = query.Where(d => d.PublicationCode.ToLower() == request.PublicationCode.ToLower());
                }

                if (!string.IsNullOrWhiteSpace(request.AuthorSearch))
                {
                    var search = request.AuthorSearch.ToLower();
                    query = query.Where(d =>
                        d.AuthorFullName.ToLower().Contains(search) ||
                        d.AuthorEmail.ToLower().Contains(search));
                }

                var result = await query
                    .OrderByDescending(d => d.CreatedAt)
                    .Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .Select(d => new DocumentSummaryDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        AuthorFullName = d.AuthorFullName,
                        AuthorEmail = d.AuthorEmail,
                        SerialCode = d.SerialCode,
                        PublicationCode = d.PublicationCode
                    })
                    .ToListAsync();

                return Ok(result);
            }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(int id, [FromBody] UpdateDocumentRequestDto dto)
        {
           var document = await _context.Documents
            .Include(d => d.DocumentPageIndex)
            .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                return NotFound(new { message = $"Documento con ID {id} no encontrado." });
            }

            if (document.Deleted || document.DeletedAt != null)
            {
                return BadRequest(new { error = "El archivo que intenta modificar ha sido eliminado." });
            }

           
            var errors = ValidateDocumentUpdate(dto);
            if (errors.Any())
                return BadRequest(new { error = errors });

            
            if (!string.IsNullOrWhiteSpace(dto.Name))
                document.Name = dto.Name;

            if (dto.Description != null)
                document.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.AuthorFullName))
                document.AuthorFullName = dto.AuthorFullName;

            if (!string.IsNullOrWhiteSpace(dto.AuthorEmail))
                document.AuthorEmail = dto.AuthorEmail;

            if (!string.IsNullOrWhiteSpace(dto.SerialCode))
                document.SerialCode = dto.SerialCode;

            if (!string.IsNullOrWhiteSpace(dto.PublicationCode))
                document.PublicationCode = dto.PublicationCode;

            document.UpdatedAt = DateTime.UtcNow;

          
            if (dto.Indexes != null)
            {
                _context.DocumentPageIndexes.RemoveRange(document.DocumentPageIndex);
                document.DocumentPageIndex = dto.Indexes.Select(i => new DocumentPageIndex
                {
                    Name = i.Name,
                    Page = i.Page,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }

            await _context.SaveChangesAsync();

            return Ok();
        }



    }
}
