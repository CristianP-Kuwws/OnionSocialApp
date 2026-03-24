using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Battleship;
using LinkUpApp.Core.Application.ViewModels.Battleship.BattleshipGame;

namespace LinkUpApp.Core.Application.Mappings.DtosAndViewModels.Battleship
{
    public class BattleshipGameViewModelMappingProfile : Profile
    {
        public BattleshipGameViewModelMappingProfile()
        {
            CreateMap<BattleshipGameDto, ActiveGameViewModel>()
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OpponentUserName, opt => opt.Ignore())
                .ForMember(dest => dest.OpponentFullName, opt => opt.Ignore())
                .ForMember(dest => dest.OpponentProfilePicture, opt => opt.Ignore())
                .ForMember(dest => dest.HoursElapsed, opt => opt.Ignore())
                .ForMember(dest => dest.MySetupStatus, opt => opt.Ignore())
                .ForMember(dest => dest.OpponentSetupStatus, opt => opt.Ignore());

            CreateMap<BattleshipGameDto, GameHistoryViewModel>()
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OpponentUserName, opt => opt.Ignore())
                .ForMember(dest => dest.OpponentFullName, opt => opt.Ignore())
                .ForMember(dest => dest.OpponentProfilePicture, opt => opt.Ignore())
                .ForMember(dest => dest.FinishedAt, opt => opt.MapFrom(src => src.FinishedAt ?? DateTime.UtcNow))
                .ForMember(dest => dest.DurationInHours, opt => opt.Ignore())
                .ForMember(dest => dest.DidIWin, opt => opt.Ignore())
                .ForMember(dest => dest.WinnerDisplay, opt => opt.Ignore());
        }
    }

}
