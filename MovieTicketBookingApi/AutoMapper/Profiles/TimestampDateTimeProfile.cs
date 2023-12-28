using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using MovieTicketBookingApi.AutoMapper.Converters;

namespace MovieTicketBookingApi.AutoMapper.Profiles;

public class TimestampDateTimeProfile : Profile
{
    public TimestampDateTimeProfile()
    {
        CreateMap<DateTime, Timestamp>().ConvertUsing<DateTimeToTimestampConverter>();
        CreateMap<Timestamp, DateTime>().ConvertUsing<TimestampToDateTimeConverter>();
    }
}
