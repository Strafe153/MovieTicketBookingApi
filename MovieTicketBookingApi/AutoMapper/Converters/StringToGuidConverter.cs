using AutoMapper;

namespace MovieTicketBookingApi.AutoMapper.Converters;

public class StringToGuidConverter : ITypeConverter<string, Guid>
{
    public Guid Convert(string source, Guid destination, ResolutionContext context) => Guid.Parse(source);
}
