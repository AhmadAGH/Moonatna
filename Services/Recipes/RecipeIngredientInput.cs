namespace Moonatna.Services.Recipes
{
    public class RecipeIngredientInput
    {
        public int? ItemId { get; set; }        // existing item, or null = create by name
        public string? Name { get; set; }
        public string? QuantityText { get; set; }
        public bool IsOptional { get; set; }
        public bool IsAdHoc { get; set; }       // the one-off toggle
        public int SortOrder { get; set; }
    }
}
