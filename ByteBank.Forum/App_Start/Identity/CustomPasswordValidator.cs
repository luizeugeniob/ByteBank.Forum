using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ByteBank.Forum.App_Start.Identity
{
    public class CustomPasswordValidator : IIdentityValidator<string>
    {
        public int LengthRequired { get; set; }
        public bool MustHaveSpecialCharacters { get; set; }
        public bool MustHaveLowerCase { get; set; }
        public bool MustHaveUpperCase { get; set; }
        public bool MustHaveNumbers { get; set; }

        public async Task<IdentityResult> ValidateAsync(string item)
        {
            var errors = new List<string>();

            if (MustHaveSpecialCharacters && !HasSpecialCharacters(item))
                errors.Add("A senha deve conter caracteres especiais!");

            if (!HasLengthRequired(item))
                errors.Add($"A senha deve conter no mínimo {LengthRequired} caracteres.");

            if (MustHaveLowerCase && !HasLowerCase(item))
                errors.Add($"A senha deve conter no mínimo uma letra minúscula.");

            if (MustHaveUpperCase && !HasUpperCase(item))
                errors.Add($"A senha deve conter no mínimo uma letra maiúscula.");

            if (MustHaveNumbers && !HasNumber(item))
                errors.Add($"A senha deve conter no mínimo um dígito.");

            if (errors.Any())
                return IdentityResult.Failed(errors.ToArray());

            return IdentityResult.Success;
        }

        private bool HasLengthRequired(string password) =>
            password?.Length >= LengthRequired;

        private bool HasSpecialCharacters(string password) =>
            Regex.IsMatch(password, @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]");

        private bool HasLowerCase(string password) =>
            password.Any(char.IsLower);

        private bool HasUpperCase(string password) =>
            password.Any(char.IsUpper);

        private bool HasNumber(string password) =>
            password.Any(char.IsDigit);
    }
}