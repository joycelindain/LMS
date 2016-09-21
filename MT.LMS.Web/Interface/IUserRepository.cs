using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MT.LMS.Web.Models;

namespace MT.LMS.Web.Interface
{
    interface IUserRepository : IDisposable
    {
        IEnumerable<User> GetUsers();
        User GetUserByID(string userID);
        void InsertUsers(User user);
        void DeleteUser(string userID);
        void UpdateUser(User user);
        void Save();
    }
}
