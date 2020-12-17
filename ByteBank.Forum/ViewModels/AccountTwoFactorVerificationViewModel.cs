using System.ComponentModel.DataAnnotations;

namespace ByteBank.Forum.ViewModels
{
    public class AccountTwoFactorVerificationViewModel
    {
        [Required]
        [Display(Name = "Token de SMS")]
        public string Token { get; set; }
        [Display(Name = "Manter logado")]
        public bool KeepLogged { get; set; }
        [Display(Name = "Lembrar deste computador")]
        public bool RememberThisComputer { get; set; }
    }
}