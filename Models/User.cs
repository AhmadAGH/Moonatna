namespace Moonatna.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirebaseUid { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
