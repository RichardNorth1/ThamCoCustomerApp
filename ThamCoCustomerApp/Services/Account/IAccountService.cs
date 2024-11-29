using ThamCoCustomerApp.Dtos;

namespace ThamCoCustomerApp.Services.Account
{
    public interface IAccountService
    {
        public Task<HttpResponseMessage> GetAccount(string accountId);
        public Task<HttpResponseMessage> CreateAccount(CustomerAccountDto account);
        public Task<HttpResponseMessage> UpdateAccount(CustomerAccountDto account);
        public Task<HttpResponseMessage> DeleteAccount(string accountId);

    }
}
