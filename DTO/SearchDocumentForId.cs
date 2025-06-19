// DTOs/DocumentDetailDto.cs
namespace TestQuantumDocs.DTOs
{
    public class DocumentPageIndexDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Page { get; set; }
    }

    public class DocumentDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AuthorFullName { get; set; }
        public string AuthorEmail { get; set; }
        public string SerialCode { get; set; }
        public string PublicationCode { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<DocumentPageIndexDto> Indexes { get; set; }
    }
}
