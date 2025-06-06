using System.ComponentModel.DataAnnotations;

namespace KapdaStore.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "👤 Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "📧 Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "🔒 Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }
    }
}
