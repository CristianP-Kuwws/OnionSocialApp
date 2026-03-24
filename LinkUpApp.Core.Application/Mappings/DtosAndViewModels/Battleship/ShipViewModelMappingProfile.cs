using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Battleship;
using LinkUpApp.Core.Application.ViewModels.Battleship.Ship;

namespace LinkUpApp.Core.Application.Mappings.DtosAndViewModels.Battleship
{
    public class ShipViewModelMappingProfile : Profile
    {
        public ShipViewModelMappingProfile()
        {
            CreateMap<PositionShipViewModel, ShipDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ShipType))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsSunk, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.PositionedAt, opt => opt.Ignore());

            // Este mapeo es algo complejo puesto que AvailableShipViewModel
            // representa tipos de barcos disponibles, no barcos individuales
        }
    }
}
