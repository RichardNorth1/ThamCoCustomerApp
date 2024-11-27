using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Diagnostics.Metrics;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using ThamCoCustomerApp.Models;

namespace ThamCoCustomerApp.Services.Account
{
    public class AccountServiceFake : IAccountService
    {
        private readonly List<CustomerAccount> _accounts;

        public AccountServiceFake()
        {
            _accounts = new List<CustomerAccount> {
                    new CustomerAccount
                    {
                        AuthId = "123",
                        Surname = "Richard",
                        Forename = "North",
                        Email = "Customer1@example.com",
                        Telephone = "01642 222222",
                        StreetAddress = "1 coronation street",
                        City = "Thornaby",
                        County = "Cleveland",
                        PostalCode = "ts177xa",
                        Balance = 100
                    },
                    new CustomerAccount
                    {
                        AuthId = "1234",
                        Surname = "john",
                        Forename = "doe",
                        Email = "Customer2@example.com",
                        Telephone = "01642 666666",
                        StreetAddress = "57 coronation street",
                        City = "Thornaby",
                        County = "Cleveland",
                        PostalCode = "ts177ka",
                        Balance = 100
                    }
                };
        }

        public Task<HttpResponseMessage> CreateAccount(CustomerAccount account)
        {
            _accounts.Add(account);

            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(account))
            });
        }

        public Task<HttpResponseMessage> DeleteAccount(string accountId)
        {
            var existingAccount = _accounts.FirstOrDefault(a => a.AuthId == accountId);

            if (existingAccount != null)
            {
                _accounts.Remove(existingAccount);

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Account deleted successfully")
                });
            }
            else
            {
                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Account not found")
                });
            }
        }

        public Task<HttpResponseMessage> GetAccount(string accountId)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(_accounts.FirstOrDefault(a => a.AuthId == accountId)))
            });
        }

        public Task<HttpResponseMessage> UpdateAccount(CustomerAccount account)
        {
            var existingAccount = _accounts.FirstOrDefault(a => a.AuthId == account.AuthId);

            if (existingAccount != null)
            {
                existingAccount.Surname = account.Surname;
                existingAccount.Forename = account.Forename;
                existingAccount.Email = account.Email;
                existingAccount.Telephone = account.Telephone;
                existingAccount.StreetAddress = account.StreetAddress;
                existingAccount.City = account.City;
                existingAccount.County = account.County;
                existingAccount.PostalCode = account.PostalCode;
                existingAccount.Balance = account.Balance;

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(existingAccount))
                });
            }
            else
            {
                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Account not found")
                });
            }
        }
    }
}
