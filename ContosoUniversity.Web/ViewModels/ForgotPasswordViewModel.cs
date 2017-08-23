using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Web.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
