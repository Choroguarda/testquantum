
using System.Text.Json.Serialization;
namespace TestQuantumDocs.Models
{

    public class DocumentPageIndex
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public int Page { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore] // ← Esto evita la referencia circular
        public Document Document { get; set; }
    }
}