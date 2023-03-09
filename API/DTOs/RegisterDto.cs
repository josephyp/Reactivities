using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        //We need a password to have atleast 1 number (?=.*\\d), one small(?=.*[a-z]), one capital(?=.*[A-Z]), min 4 and max 8 character length
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Password must be complex")]
        public string Password { get; set; }

        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string Username { get; set; }
    }
}