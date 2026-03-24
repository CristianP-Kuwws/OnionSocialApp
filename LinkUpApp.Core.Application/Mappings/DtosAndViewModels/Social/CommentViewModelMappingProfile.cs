using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Social;
using LinkUpApp.Core.Application.ViewModels.Social.Comment;

namespace LinkUpApp.Core.Application.Mappings.DtosAndViewModels.Social
{
    public class CommentViewModelMappingProfile : Profile
    {
        public CommentViewModelMappingProfile()
        {
            CreateMap<CommentDto, SaveCommentViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (int?)src.Id))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 0))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Este mapeo requiere datos adicionales del usuario que no estan en CommentDto
            // En mi caso opte por hacerlo de forma manual en el controller
            CreateMap<CommentDto, CommentViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.UserFullName, opt => opt.Ignore())
                .ForMember(dest => dest.UserProfilePicture, opt => opt.Ignore())
                .ForMember(dest => dest.IsOwner, opt => opt.Ignore())
                .ForMember(dest => dest.Replies, opt => opt.Ignore());
        }
    }
}
