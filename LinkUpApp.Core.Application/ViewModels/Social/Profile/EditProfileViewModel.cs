using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Application.ViewModels.Social.User
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(60)]
        [Display(Name = "Nombre")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido.")]
        [StringLength(60)]
        [Display(Name = "Apellido")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido.")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        [Display(Name = "Teléfono")]
        public required string Phone { get; set; }

        [Display(Name = "Foto de Perfil")]
        public IFormFile? ProfilePicture { get; set; }

        public string? CurrentProfilePicturePath { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Nueva Contraseña")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Contraseña")]
        public string? ConfirmPassword { get; set; }
    }
}
