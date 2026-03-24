using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Application.ViewModels.Login
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(60)]
        [Display(Name = "Nombre")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido.")]
        [StringLength(60)]
        [Display(Name = "Apellido")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "El telefono es requerido.")]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Formato: 809-123-4567")]
        [Display(Name = "Telefono")]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "El correo es requerido.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es valido.")]
        [Display(Name = "Correo Electronico")]
        public required string Email { get; set; }

        public string? ProfilePicturePath { get; set; }

        [Required(ErrorMessage = "La foto de perfil es requerida.")]
        [Display(Name = "Foto de Perfil")]
        public IFormFile? ProfilePicture { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [Display(Name = "Nombre de Usuario")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public required string ConfirmPassword { get; set; }
    }    

}
