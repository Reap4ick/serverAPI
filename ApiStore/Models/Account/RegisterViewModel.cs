using System.ComponentModel.DataAnnotations;

namespace ApiStore.Models.Account
{
    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6, ErrorMessage = "Пароль має містити мінімум 6 символів")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ім'я є обов'язковим")]
        public string Firstname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Прізвище є обов'язковим")]
        public string Lastname { get; set; } = string.Empty;
    }
}
