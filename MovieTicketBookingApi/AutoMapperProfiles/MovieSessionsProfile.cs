using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using MovieTicketBookingApi.Extensions;
using MovieTicketBookingApi.Protos.MovieSessions;
using CoreMovieSession = Core.Entities.MovieSession;

namespace MovieTicketBookingApi.AutoMapperProfiles
{
    public class MovieSessionsProfile : Profile
    {
        public MovieSessionsProfile()
        {
            CreateMap<IList<CoreMovieSession>, GetAllMovieSessionsReply>()
                .ForMember(r => r.MovieSessions, opt => opt.MapFrom(l => l.ToRepeatedField()));

            CreateMap<CoreMovieSession, GetMovieSessionByIdReply>()
                .ForMember(r => r.Id, opt => opt.MapFrom(s => s.Id.ToString()))
                .ForMember(r => r.DateTime, opt => opt.MapFrom(s => Timestamp.FromDateTime(s.DateTime)))
                .ForMember(r => r.MovieId, opt => opt.MapFrom(s => s.MovieId.ToString()))
                .ForMember(r => r.MovieHallId, opt => opt.MapFrom(s => s.MovieHallId.ToString()))
                .ForMember(r => r.Tickets, opt => opt.MapFrom(s => s.Tickets.ToRepeatedField()));

            CreateMap<CreateMovieSessionRequest, CoreMovieSession>()
                .ForMember(s => s.DateTime, opt => opt.MapFrom(r => r.DateTime.ToDateTime()))
                .ForMember(s => s.MovieId, opt => opt.MapFrom(r => new Guid(r.MovieId)))
                .ForMember(s => s.MovieHallId, opt => opt.MapFrom(r => new Guid(r.MovieHallId)));

            CreateMap<CoreMovieSession, CreateMovieSessionReply>()
                .ForMember(r => r.Id, opt => opt.MapFrom(s => s.Id.ToString()))
                .ForMember(r => r.DateTime, opt => opt.MapFrom(s => Timestamp.FromDateTime(s.DateTime)))
                .ForMember(r => r.MovieId, opt => opt.MapFrom(s => s.MovieId.ToString()))
                .ForMember(r => r.MovieHallId, opt => opt.MapFrom(s => s.MovieHallId.ToString()));

            CreateMap<UpdateMovieSessionRequest, CoreMovieSession>()
                .ForMember(s => s.DateTime, opt => opt.MapFrom(r => r.DateTime.ToDateTime()))
                .ForMember(s => s.Id, opt => opt.Ignore());
        }
    }
}
