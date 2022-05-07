using PagedList.Core;
using TSDC.Core.Domain.Master;

namespace TSDC.Service.Master
{
    public interface IOrganizationService
    {
        Task InsertAsync(Organization entity);

        void UpdateAsync(Organization entity);

        void DeleteAsync(Organization entity);

        Task<bool> ExistsAsync(string code);

        Task<Organization> GetByIdAsync(int id);

        IPagedList<Organization> Get(OrganizationSearchContext ctx);
    }
}
