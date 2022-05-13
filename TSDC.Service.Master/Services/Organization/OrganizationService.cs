using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using TSDC.Core.Domain.Master;
using TSDC.Core.Domain.Master.Enums;

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
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _masterContext.Organization select p;

            if (!string.IsNullOrEmpty(ctx.Keywords))
            {
                query = from p in query
                        where p.Code.Contains(ctx.Keywords) ||
                              p.Name.Contains(ctx.Keywords)
                        select p;
            }

            if (ctx.Status != null)
            {
                if (ctx.Status == (int)ActiveStatus.Active)
                {
                    query = from p in query
                            where !p.Inactive
                            select p;
                }

                if (ctx.Status == (int)ActiveStatus.Inactive)
                {
                    query = from p in query
                            where p.Inactive
                            select p;
                }
            }

            query = from p in query
                    orderby p.Code
                    select p;

            return new PagedList<Organization>(query, ctx.PageNumber, ctx.PageSize);
        }
        #endregion
    }
}
