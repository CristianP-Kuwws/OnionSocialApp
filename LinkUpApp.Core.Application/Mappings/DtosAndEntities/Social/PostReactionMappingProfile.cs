using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Social;
using LinkUpApp.Core.Domain.Entities.Social;

namespace LinkUpApp.Core.Application.Mappings.DtosAndEntities.Social
{
    public class PostReactionMappingProfile : Profile
    {
        public PostReactionMappingProfile()
        {
            CreateMap<PostReaction, PostReactionDto>()
                .ReverseMap()
                .ForMember(dest => dest.Post, opt => opt.Ignore());
        }
    }
}
