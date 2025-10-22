namespace JWT.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string? Value { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
