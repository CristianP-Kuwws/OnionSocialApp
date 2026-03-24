using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Application.ViewModels.Login
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [Display(Name = "Usuario")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "La contraseña  es requerida.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public required string Password { get; set; } = string.Empty;
    }

}
