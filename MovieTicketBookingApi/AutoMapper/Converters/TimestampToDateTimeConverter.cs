using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace MovieTicketBookingApi.AutoMapper.Converters;

public class TimestampToDateTimeConverter : ITypeConverter<Timestamp, DateTime>
{
    public DateTime Convert(Timestamp source, DateTime destination, ResolutionContext context) => source.ToDateTime();
}
