using AutoMapper;
using MovieTicketBookingApi.AutoMapper.Converters;

namespace MovieTicketBookingApi.AutoMapper.Profiles;

public class GuidStringProfile : Profile
{
	public GuidStringProfile()
	{
		CreateMap<Guid, string>().ConvertUsing<GuidToStringConverter>();
		CreateMap<string, Guid>().ConvertUsing<StringToGuidConverter>();
	}
}
