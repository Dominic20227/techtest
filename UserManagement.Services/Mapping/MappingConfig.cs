using AutoMapper;
using UserManagement.Common.LogsModels;
using UserManagement.Common.UserModels;
using UserManagement.Data.Entities;

namespace UserManagement.Services.Mapping;
public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<User, UserModel>().ReverseMap();
        CreateMap<User, AddUserModel>().ReverseMap();
        CreateMap<User, UpdateUserModel>().ReverseMap();
        CreateMap<User, UserLogsModel>().ReverseMap();

        CreateMap<Logs, LogsModel>().ReverseMap();

    }
}
