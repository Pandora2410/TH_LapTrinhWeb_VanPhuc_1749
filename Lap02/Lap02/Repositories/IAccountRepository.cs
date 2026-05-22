using Lap02.Models;

namespace Lap02.Repositories
{
    public interface IAccountRepository
    {
        UserAccount? Login(string username, string password);
        List<UserAccount> GetAllAccounts();
    }
}
