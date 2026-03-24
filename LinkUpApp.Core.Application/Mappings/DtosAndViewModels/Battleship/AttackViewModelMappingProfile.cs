using AutoMapper;
using LinkUpApp.Core.Application.Dtos.Battleship;
using LinkUpApp.Core.Application.ViewModels.Battleship.Attack;

namespace LinkUpApp.Core.Application.Mappings.DtosAndViewModels.Battleship
{
    public class AttackViewModelMappingProfile : Profile
    {
        public AttackViewModelMappingProfile()
        {
            CreateMap<AttackViewModel, AttackDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AttackingUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Result, opt => opt.Ignore())
                .ForMember(dest => dest.TurnNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<AttackDto, AttackViewModel>();
        }
    }
}
