using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Security.Cryptography;
using TSDC.Core.Domain.Master;

namespace TSDC.Service.Master
{
    public class UserService : IUserService
    {
        #region Fields
        private readonly MasterContext _masterContext;
        #endregion

        #region Ctor
        public UserService(
            MasterContext masterContext)
        {
            _masterContext = masterContext;
        }
        #endregion

        #region Methods
        public async Task InsertAsync(User entity, string password)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            entity.PasswordHash = passwordHash;
            entity.PasswordSalt = passwordSalt;

            await _masterContext.User.AddAsync(entity);
            _masterContext.SaveChanges();
        }        

        public void UpdateAsync(User entity, string password)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (!string.IsNullOrEmpty(password))
            {
                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                entity.PasswordHash = passwordHash;
                entity.PasswordSalt = passwordSalt;
            }
            
            _masterContext.User.Update(entity);
            _masterContext.SaveChanges();
        }

        public void DeleteAsync(User entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }            

            _masterContext.User.Remove(entity);
            _masterContext.SaveChanges();
        }

        public async Task<bool> ExistsAsync(string code, string userName, string email)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(code) + " or " + nameof(userName) + " or " + nameof(email));
            }

            return await _masterContext.User.AnyAsync(
                x => x.Code != null &&
                     x.UserName != null &&
                     x.Email != null &&
                     x.Code.Equals(code) &&
                     x.UserName.Equals(userName) &&
                     x.Email.Equals(email));
        }

        public async Task<User> GetByIdAsync(int id)
        {
            if (id == 0)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await _masterContext.User.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> Authentication(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(userName) + " or " + nameof(password));
            }

            var user = await _masterContext.User.FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null)
            {
                return null;
            }

            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }
        #endregion

        #region List
        public IPagedList<User> Get(UserSearchContext ctx)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Utilities
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (passwordHash.Length != 64)
            {
                throw new ArgumentNullException(nameof(passwordHash));
            }

            if (passwordSalt.Length != 128)
            {
                throw new ArgumentNullException(nameof(passwordSalt));
            }

            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion
    }
}
