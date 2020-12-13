using System.ComponentModel.DataAnnotations;

namespace ByteBank.Forum.ViewModels
{
    public class AccountLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Manter logado")]
        public bool KeepLogged { get; set; }
    }
}