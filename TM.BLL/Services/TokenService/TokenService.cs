using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.DAL;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IBaseRepository<Token> _tokenRepository;
        private readonly Context _context;
        public TokenService(IBaseRepository<Token> tokenRepository, Context context)
        {
            _tokenRepository = tokenRepository;
            _context = context;
        }
        public async Task<Token> CreateTokenAsync(Token token)
        {
            await _tokenRepository.AddAsync(token);
            return token;
        }

        public async Task<Token> DeactiveToken(int tokenId)
        {
            var existedToken = await _tokenRepository.GetByIdAsync(tokenId);
            if (existedToken == null)
                throw new KeyNotFoundException($"Token with id {tokenId} not found.");
            existedToken.IsActive = false;
            await _tokenRepository.UpdateAsync(existedToken);
            return existedToken;
        }
    }
}
