using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MT.LMS.Web.Interface;
using MT.LMS.Web.Models;
using System.Data.Entity;
namespace MT.LMS.Web.Repository
{
    public class UserRepository : IUserRepository
    {
        private LMSDBContext _dbContext;

        public UserRepository(LMSDBContext userContext)
        {
            _dbContext = userContext;
        }
               

        public void DeleteUser(string userID)
        {
            User user = _dbContext.Users.Find(userID);
            _dbContext.Users.Remove(user);
        }

        public IEnumerable<User> GetUsers()
        {
            return _dbContext.Users.ToList();
        }

        public void InsertUsers(User user)
        {
            _dbContext.Users.Add(user);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _dbContext.Dispose();
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        public User GetUserByID(string userID)
        {
            return _dbContext.Users.Find(userID);
        }
        #endregion



    }
}