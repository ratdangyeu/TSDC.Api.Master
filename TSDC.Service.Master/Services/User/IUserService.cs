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

        Task<string> Authentication(string userName, string password);

        IPagedList<User> Get(UserSearchContext ctx);
    }
}
