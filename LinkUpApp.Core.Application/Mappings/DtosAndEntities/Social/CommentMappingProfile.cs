using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Social;
using LinkUpApp.Core.Domain.Entities.Social;

namespace LinkUpApp.Core.Application.Mappings.DtosAndEntities.Social
{
    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile()
        {
            CreateMap<Comment, CommentDto>()
                .ReverseMap()
                .ForMember(dest => dest.Post, opt => opt.Ignore())
                .ForMember(dest => dest.ParentComment, opt => opt.Ignore())
                .ForMember(dest => dest.Replies, opt => opt.Ignore());
        }
    }
}
