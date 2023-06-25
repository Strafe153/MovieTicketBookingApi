using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using MovieTicketBookingApi.Extensions;
using MovieTicketBookingApi.Protos.V1.Users;
using CoreUser = Core.Entities.User;

namespace MovieTicketBookingApi.AutoMapperProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<IList<CoreUser>, GetAllUsersReply>()
            .ForMember(r => r.Users, opt => opt.MapFrom(u => u.ToRepeatedField()));

        CreateMap<CoreUser, GetUserbyIdReply>()
            .ForMember(r => r.Id, opt => opt.MapFrom(u => u.Id.ToString()))
            .ForMember(r => r.BirthDate, opt => opt.MapFrom(u => Timestamp.FromDateTime(u.BirthDate)));

        CreateMap<UpdateUserRequest, CoreUser>()
            .ForMember(u => u.BirthDate, opt => opt.MapFrom(r => r.BirthDate.ToDateTime()))
            .ForMember(u => u.Id, opt => opt.Ignore());
    }
}
