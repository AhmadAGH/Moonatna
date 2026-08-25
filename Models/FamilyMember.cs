namespace Moonatna.Models
{
    public enum FamilyRole : byte
    {
        Owner = 0,
        Member = 1
    }

    public class FamilyMember
    {
        public int Id { get; set; }
        public int FamilyId { get; set; }
        public int UserId { get; set; }
        public FamilyRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
