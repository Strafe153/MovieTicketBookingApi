using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using MovieTicketBookingApi.Protos.Movies;

namespace MovieTicketBookingApi.Extensions;

public static class IListExtensions
{
    public static RepeatedField<MovieSession> ToRepeatedField(this IList<Core.Entities.MovieSession> incoming)
    {
        var result = new RepeatedField<MovieSession>();

        foreach (var item in incoming)
        {
            result.Add(new MovieSession
            {
                Id = item.Id.ToString(),
                DateTime = Timestamp.FromDateTime(item.DateTime)
            });
        }

        return result;
    }
}
