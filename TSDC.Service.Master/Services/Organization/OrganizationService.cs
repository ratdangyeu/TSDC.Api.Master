using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using TSDC.Core.Domain.Master;

namespace TSDC.Service.Master
{
    public class OrganizationService : IOrganizationService
    {
        #region Fields
        private readonly MasterContext _masterContext;
        #endregion

        #region Ctor
        public OrganizationService(
            MasterContext masterContext)
        {
            _masterContext = masterContext;
        }
        #endregion

        #region Methods
        public async Task InsertAsync(Organization entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _masterContext.Organization.AddAsync(entity);
            _masterContext.SaveChanges();
        }

        public void UpdateAsync(Organization entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _masterContext.Organization.Update(entity);
            _masterContext.SaveChanges();
        }

        public void DeleteAsync(Organization entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _masterContext.Organization.Remove(entity);
            _masterContext.SaveChanges();
        }

        public async Task<bool> ExistsAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            return await _masterContext.Organization.AnyAsync(
                x => x.Code != null &&
                     x.Code.Equals(code));
        }

        public async Task<Organization> GetByIdAsync(int id)
        {
            if (id == 0)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await _masterContext.Organization.FirstOrDefaultAsync(x => x.Id == id);
        }
        #endregion

        #region List
        public IPagedList<Organization> Get(OrganizationSearchContext ctx)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
