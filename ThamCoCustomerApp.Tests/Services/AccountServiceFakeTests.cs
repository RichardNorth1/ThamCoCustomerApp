using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Services.Account;

namespace ThamCoCustomerApp.Tests.Services
{
    [TestFixture]
    public class AccountServiceFakeTests
    {
        private AccountServiceFake _accountServiceFake;

        [SetUp]
        public void SetUp()
        {
            _accountServiceFake = new AccountServiceFake();
        }

        [Test]
        public async Task CreateAccount_ShouldAddAccount()
        {
            var newAccount = new CustomerAccountDto
            {
                AuthId = "5678",
                Surname = "Smith",
                Forename = "Jane",
                Email = "Customer3@example.com",
                Telephone = "01642 777777",
                StreetAddress = "99 coronation street",
                City = "Thornaby",
                County = "Cleveland",
                PostalCode = "ts177la",
                Balance = 200
            };

            var response = await _accountServiceFake.CreateAccount(newAccount);
            var createdAccount = JsonSerializer.Deserialize<CustomerAccountDto>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsNotNull(createdAccount);
            Assert.That(createdAccount.AuthId, Is.EqualTo(newAccount.AuthId));
            Assert.That((await _accountServiceFake.GetAccounts()).Content.ReadAsStringAsync().Result.Contains(newAccount.AuthId));
        }

        [Test]
        public async Task DeleteAccount_ShouldRemoveAccount_WhenAccountExists()
        {
            var accountId = "123";

            var response = await _accountServiceFake.DeleteAccount(accountId);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.AreEqual("Account deleted successfully", content);

            var accountsResponse = await _accountServiceFake.GetAccounts();
            var accounts = JsonSerializer.Deserialize<List<CustomerAccountDto>>(await accountsResponse.Content.ReadAsStringAsync());
            Assert.IsFalse(accounts.Any(a => a.AuthId == accountId));
        }


        [Test]
        public async Task DeleteAccount_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            var accountId = "999";

            var response = await _accountServiceFake.DeleteAccount(accountId);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.AreEqual("Account not found", content);
        }

        [Test]
        public async Task GetAccount_ShouldReturnAccount_WhenAccountExists()
        {
            var accountId = "123";

            var response = await _accountServiceFake.GetAccount(accountId);
            var account = JsonSerializer.Deserialize<CustomerAccountDto>(await response.Content.ReadAsStringAsync());

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsNotNull(account);
            Assert.That(account.AuthId, Is.EqualTo(accountId));
        }

        [Test]
        public async Task GetAccount_ShouldReturnNull_WhenAccountDoesNotExist()
        {
            var accountId = "999";

            var response = await _accountServiceFake.GetAccount(accountId);
            var account = JsonSerializer.Deserialize<CustomerAccountDto>(await response.Content.ReadAsStringAsync());

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsNull(account);
        }

        [Test]
        public async Task UpdateAccount_ShouldUpdateAccount_WhenAccountExists()
        {
            var updatedAccount = new CustomerAccountDto
            {
                AuthId = "123",
                Surname = "UpdatedSurname",
                Forename = "UpdatedForename",
                Email = "UpdatedEmail@example.com",
                Telephone = "01642 888888",
                StreetAddress = "Updated address",
                City = "Updated city",
                County = "Updated county",
                PostalCode = "Updated postal code",
                Balance = 100
            };

            var response = await _accountServiceFake.UpdateAccount(updatedAccount);
            var account = JsonSerializer.Deserialize<CustomerAccountDto>(await response.Content.ReadAsStringAsync());

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsNotNull(account);
            Assert.That(account.Surname, Is.EqualTo(updatedAccount.Surname));
            Assert.That(account.Forename, Is.EqualTo(updatedAccount.Forename));
            Assert.That(account.Email, Is.EqualTo(updatedAccount.Email));
            Assert.That(account.Telephone, Is.EqualTo(updatedAccount.Telephone));
            Assert.That(account.StreetAddress, Is.EqualTo(updatedAccount.StreetAddress));
            Assert.That(account.City, Is.EqualTo(updatedAccount.City));
            Assert.That(account.County, Is.EqualTo(updatedAccount.County));
            Assert.That(account.PostalCode, Is.EqualTo(updatedAccount.PostalCode));
            Assert.That(account.Balance, Is.EqualTo(updatedAccount.Balance));
        }

        [Test]
        public async Task UpdateAccount_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            var updatedAccount = new CustomerAccountDto
            {
                AuthId = "999",
                Surname = "UpdatedSurname",
                Forename = "UpdatedForename",
                Email = "UpdatedEmail@example.com",
                Telephone = "01642 888888",
                StreetAddress = "Updated address",
                City = "Updated city",
                County = "Updated county",
                PostalCode = "Updated postal code",
                Balance = 300
            };

            var response = await _accountServiceFake.UpdateAccount(updatedAccount);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.AreEqual("Account not found", content);
        }
    }
}
