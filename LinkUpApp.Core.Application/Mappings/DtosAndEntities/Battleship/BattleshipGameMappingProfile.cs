using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Battleship;
using LinkUpApp.Core.Domain.Entities.Battleship;

namespace LinkUpApp.Core.Application.Mappings.DtosAndEntities.Battleship
{
    public class BattleshipGameMappingProfile : Profile
    {
        public BattleshipGameMappingProfile()
        {
            CreateMap<BattleshipGame, BattleshipGameDto>()
                .ReverseMap()
                .ForMember(dest => dest.Ships, opt => opt.Ignore())
                .ForMember(dest => dest.Attacks, opt => opt.Ignore());
        }
    }
}
