using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace MovieTicketBookingApi.AutoMapper.Converters;

public class DateTimeToTimestampConverter : ITypeConverter<DateTime, Timestamp>
{
    public Timestamp Convert(DateTime source, Timestamp destination, ResolutionContext context) => Timestamp.FromDateTime(source);
}
