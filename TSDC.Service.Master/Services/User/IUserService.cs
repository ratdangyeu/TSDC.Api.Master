using PagedList.Core;
using TSDC.Core.Domain.Master;

namespace TSDC.Service.Master
{
    public interface IUserService
    {
        Task InsertAsync(User entity, string password);

        void UpdateAsync(User entity, string password);

        void DeleteAsync(User entity);

        Task<bool> ExistsAsync(string code, string userName, string email);

        Task<User> GetByIdAsync(int id);

        Task<User?> Authentication(string userName, string password);

        string GenerateJwtToken(User user);

        IPagedList<User> Get(UserSearchContext ctx);
    }
}
