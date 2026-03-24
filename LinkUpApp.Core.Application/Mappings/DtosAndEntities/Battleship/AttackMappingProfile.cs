using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Battleship;
using LinkUpApp.Core.Domain.Entities.Battleship;

namespace LinkUpApp.Core.Application.Mappings.DtosAndEntities.Battleship
{
    public class AttackMappingProfile : Profile
    {
        public AttackMappingProfile()
        {
            CreateMap<Attack, AttackDto>()
                .ReverseMap()
                // ignorar navigation property Game al mapear de DTO a Entity
                .ForMember(dest => dest.Game, opt => opt.Ignore());
        }
    }
}
