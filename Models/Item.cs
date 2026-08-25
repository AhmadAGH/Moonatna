namespace Moonatna.Models
{
    public enum ItemState : byte
    {
        Available = 0,
        RunningLow = 1,
        OutOfStock = 2
    }

    public class Item
    {
        public int Id { get; set; }
        public int FamilyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public ItemState State { get; set; }
        public bool IsAdHoc { get; set; }
        public string? ImagePath { get; set; }
        public int CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsArchived { get; set; }
    }

}
