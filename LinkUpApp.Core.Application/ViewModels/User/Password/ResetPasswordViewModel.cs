using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Application.ViewModels.Login.Password
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public required string Token { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Text)]
        public required string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Nueva Contraseña")]
        public required string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public required string ConfirmPassword { get; set; } = string.Empty;
    }
}
