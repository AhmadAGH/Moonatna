using Moonatna.Models;

namespace Moonatna.ViewModels.Orgnize;

public class OrganizeIndexViewModel
{
    public int FamilyId { get; set; }
    public List<Category> Categories { get; set; } = new();   // via ILookupsRepository ONLY
    public List<OrganizeItemViewModel> Items { get; set; } = new();
}

public class OrganizeItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
}
