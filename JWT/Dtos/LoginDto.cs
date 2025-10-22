using System.ComponentModel.DataAnnotations;

namespace JWT.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        public string UserName { get; set; } = null!;


        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*\d).{6,}$", ErrorMessage = "Password must contain at least one digit")]
        public string Password { get; init; } = null!;
    }
}
