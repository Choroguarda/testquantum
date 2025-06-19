namespace TestQuantumDocs.DTOs
{
    public class SearchDocumentsRequestDto
    {
        public int? Id { get; set; } // Opcional
        public string? SerialCode { get; set; } // Opcional
        public string? PublicationCode { get; set; } // Opcional
        public string? AuthorSearch { get; set; } // Opcional

        public int Page { get; set; } = 1; // Por defecto 1
    }
}
