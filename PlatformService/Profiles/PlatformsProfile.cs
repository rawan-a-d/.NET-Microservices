using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

// AutoMapper
namespace PlatformService.Profiles
{
	public class PlatformsProfile : Profile
	{
		public PlatformsProfile()
		{
			// Source -> Target
			CreateMap<Platform, PlatformReadDto>();
			CreateMap<PlatformCreateDto, Platform>();
			CreateMap<PlatformReadDto, PlatformPublishedDto>();
			// grpc
			CreateMap<Platform, GrpcPlatformModel>()
				.ForMember(dest => dest.PlatformId, opt =>
					opt.MapFrom(src => src.Id)
				)
				// optional
				.ForMember(dest => dest.Name, opt =>
					opt.MapFrom(src => src.Name)
				)
				.ForMember(dest => dest.Publisher, opt =>
					opt.MapFrom(src => src.Publisher)
				);
		}
	}
}