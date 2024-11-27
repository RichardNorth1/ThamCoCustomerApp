using ThamCoCustomerApp.Dtos;

namespace ThamCoCustomerApp.Services.Token
{
    public interface ITokenService
    {
        public Task<TokenDto> GetToken();
    }
}
