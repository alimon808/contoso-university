using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Web.ViewModels
{
    public class TokenViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}