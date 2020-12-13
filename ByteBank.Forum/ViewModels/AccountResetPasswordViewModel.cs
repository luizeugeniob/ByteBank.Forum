using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ByteBank.Forum.ViewModels
{
    public class AccountResetPasswordViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string UserId { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NewPassword { get; set; }
    }
}