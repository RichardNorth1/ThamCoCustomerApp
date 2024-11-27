using ThamCoCustomerApp.Models;

namespace ThamCoCustomerApp.Services.Account
{
    public interface IAccountService
    {
        public Task<HttpResponseMessage> GetAccount(string accountId);
        public Task<HttpResponseMessage> CreateAccount(CustomerAccount account);
        public Task<HttpResponseMessage> UpdateAccount(CustomerAccount account);
        public Task<HttpResponseMessage> DeleteAccount(string accountId);

    }
}
