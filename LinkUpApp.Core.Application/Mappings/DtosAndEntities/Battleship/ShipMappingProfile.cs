using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Battleship;
using LinkUpApp.Core.Domain.Entities.Battleship;

namespace LinkUpApp.Core.Application.Mappings.DtosAndEntities.Battleship
{
    public class ShipMappingProfile : Profile
    {
        public ShipMappingProfile()
        {
            CreateMap<Ship, ShipDto>()
                .ReverseMap()
                
                .ForMember(dest => dest.Game, opt => opt.Ignore());
        }
    }
}
