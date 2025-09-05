using AutoMapper;
using Dtos;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TM.BLL.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile() {

            CreateMap<RegisterDto, User>();
            CreateMap<User, RegisterDto>();
        }
    }
}
