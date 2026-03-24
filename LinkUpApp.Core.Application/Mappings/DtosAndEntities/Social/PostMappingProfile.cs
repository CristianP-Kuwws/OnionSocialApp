using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Social;
using LinkUpApp.Core.Domain.Entities.Social;

namespace LinkUpApp.Core.Application.Mappings.DtosAndEntities.Social
{
    public class PostMappingProfile : Profile
    {
        public PostMappingProfile()
        {
            CreateMap<Post, PostDto>()
                .ReverseMap()
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Reactions, opt => opt.Ignore());
        }
    }
}
