namespace Moonatna.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public int FamilyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Steps { get; set; }
        public string? PhotoPath { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsArchived { get; set; }
    }
}
