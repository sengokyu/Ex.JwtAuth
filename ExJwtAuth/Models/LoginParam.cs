using System.ComponentModel.DataAnnotations;

namespace ExJwtAuth.Models
{
    public class LoginParam
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}