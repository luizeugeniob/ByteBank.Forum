using System.ComponentModel.DataAnnotations;

namespace ByteBank.Forum.ViewModels
{
    public class AccountForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}