using AutoMapper;
using MovieTicketBookingApi.Protos.V1.Users;
using CoreUser = Core.Entities.User;

namespace MovieTicketBookingApi.AutoMapperProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CoreUser, User>();

        CreateMap<IList<CoreUser>, GetAllUsersReply>()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src));

        CreateMap<CoreUser, GetUserbyIdReply>();

        CreateMap<RegisterUserRequest, CoreUser>();

        CreateMap<CoreUser, RegisterUserReply>();

        CreateMap<UpdateUserRequest, CoreUser>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
