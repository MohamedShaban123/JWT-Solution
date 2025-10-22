using Microsoft.AspNetCore.Identity;

namespace JWT.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    }

}
