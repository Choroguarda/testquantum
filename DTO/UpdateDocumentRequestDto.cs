using TestQuantumDocs.DTOs;

public class UpdateDocumentRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? AuthorFullName { get; set; }
    public string? AuthorEmail { get; set; }
    public string? SerialCode { get; set; }
    public string? PublicationCode { get; set; }
     public List<CreateDocumentPageIndexDto>? Indexes { get; set; }
}
