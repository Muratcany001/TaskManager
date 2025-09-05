using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.TokenService
{
    public interface ITokenService
    {
        Task<Token> CreateTokenAsync (Token token);
        Task<Token> DeactiveToken(int tokenId);
    }
}
