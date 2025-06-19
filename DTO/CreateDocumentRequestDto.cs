namespace TestQuantumDocs.DTOs
{
    public class CreateDocumentPageIndexDto
    {
        public string Name { get; set; }
        public int Page { get; set; }
    }

    public class CreateDocumentRequestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string AuthorFullName { get; set; }
        public string AuthorEmail { get; set; }
        public string SerialCode { get; set; }
        public string PublicationCode { get; set; }

        public List<CreateDocumentPageIndexDto> Indexes { get; set; } = new();


    }
}
