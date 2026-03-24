using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Social;
using LinkUpApp.Core.Domain.Entities.Social;

namespace LinkUpApp.Core.Application.Mappings.DtosAndEntities.Social
{
    public class FriendshipMappingProfile : Profile
    {
        public FriendshipMappingProfile()
        {
            CreateMap<Friendship, FriendshipDto>()
                .ReverseMap();

        }
    }
}
