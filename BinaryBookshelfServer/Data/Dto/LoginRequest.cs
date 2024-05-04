using System.ComponentModel.DataAnnotations;

namespace BinaryBookshelfServer.Data.Dto
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
    }
}
