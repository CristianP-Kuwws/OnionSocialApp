using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Application.ViewModels.Login.Password
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [Display(Name = "Nombre de Usuario")]
        public required string UserName { get; set; } = string.Empty;
    }

}
