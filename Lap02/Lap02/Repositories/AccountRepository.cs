using Lap02.Models;

namespace Lap02.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly List<UserAccount> _accounts = new List<UserAccount>
        {
            new UserAccount
            {
                Username = "admin",
                Password = "123",
                Role = "Admin"
            },
            new UserAccount
            {
                Username = "user",
                Password = "123",
                Role = "User"
            }
        };

        public UserAccount? Login(string username, string password)
        {
            return _accounts.FirstOrDefault(x =>
                x.Username == username &&
                x.Password == password);
        }

        public List<UserAccount> GetAllAccounts()
        {
            return _accounts;
        }
    }
}