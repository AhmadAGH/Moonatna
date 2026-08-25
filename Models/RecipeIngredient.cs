namespace Moonatna.Models
{
    public class RecipeIngredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int ItemId { get; set; }
        public string? QuantityText { get; set; }
        public bool IsOptional { get; set; }
        public int SortOrder { get; set; }
    }
}
