using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Application.ViewModels.Social.Posts
{
    public class SavePostViewModel
    {
        public int? Id { get; set; } // Null si es nueva, con valor si es edicion

        [Required(ErrorMessage = "El contenido es requerido.")]
        [StringLength(5000)]
        [Display(Name = "Contenido")]
        public required string Content { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el tipo de publicacion.")]
        [Display(Name = "Tipo de Contenido")]
        public required string MediaType { get; set; } // "Imagen" o "Video"

        [Display(Name = "Imagen")]
        public IFormFile? ImageFile { get; set; }

        [Url(ErrorMessage = "La URL de YouTube no es valida.")]
        [Display(Name = "Enlace de YouTube")]
        public string? YouTubeUrl { get; set; }
    }
}
