using Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.BLL.ViewModels;

namespace TM.BLL.Services.AuthService
{
    public interface IAuthService
    {
        Task<ResultViewModel<string>> Login (LoginDto loginDto);
    }
}
