namespace Moonatna.Models
{
    public class FamilyMemberInfo
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public FamilyRole Role { get; set; }
    }
}
