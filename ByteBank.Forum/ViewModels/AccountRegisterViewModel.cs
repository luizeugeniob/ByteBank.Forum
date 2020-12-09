using System.ComponentModel.DataAnnotations;

namespace ByteBank.Forum.ViewModels
{
    public class AccountRegisterViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Nome completo")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Senha")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}