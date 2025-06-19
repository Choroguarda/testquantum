namespace TestQuantumDocs.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AuthorFullName { get; set; }
        public string AuthorEmail { get; set; }
        public string SerialCode { get; set; }
        public string PublicationCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public bool IsDeleted => DeletedAt.HasValue;

        public ICollection<DocumentPageIndex> DocumentPageIndex { get; set; }
    }
}
