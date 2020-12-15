using System.ComponentModel.DataAnnotations;

namespace ByteBank.Forum.ViewModels
{
    public class AccountMyAccountViewModel
    {
        [Required]
        [Display(Name = "Nome completo")]
        public string FullName { get; set; }
        [Display(Name = "Celular")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Dois fatores habilitados")]
        public bool TwoFactorEnabled { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
    }
}