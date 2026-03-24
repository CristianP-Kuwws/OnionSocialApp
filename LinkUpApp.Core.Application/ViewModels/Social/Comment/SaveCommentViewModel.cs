using System.ComponentModel.DataAnnotations;

namespace LinkUpApp.Core.Application.ViewModels.Social.Comment
{
    public class SaveCommentViewModel
    {
        public int? Id { get; set; }
        public required int PostId { get; set; }
        public int? ParentCommentId { get; set; }

        [Required(ErrorMessage = "El contenido es requerido.")]
        [StringLength(2000)]
        [Display(Name = "Comentario")]
        public required string Content { get; set; }
    }
}
