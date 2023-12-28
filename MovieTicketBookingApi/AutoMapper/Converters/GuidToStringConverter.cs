using AutoMapper;

namespace MovieTicketBookingApi.AutoMapper.Converters;

public class GuidToStringConverter : ITypeConverter<Guid, string>
{
    public string Convert(Guid source, string destination, ResolutionContext context) => source.ToString();
}
