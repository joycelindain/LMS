using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MT.LMS.Web.Models;
using MT.LMS.Web.Repository;
using MT.LMS.Web.Interface;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Text;

namespace MT.LMS.Web.Controllers
{
    public class UsersController : Controller
    {
        private LMSDBContext db = new LMSDBContext();
        private IUserRepository _userRepository;
        private const int saltLengthLimit = 24;
        public UsersController()
        {
            _userRepository = new UserRepository(new LMSDBContext());
        }
        #region CRUD
        // GET: Users
        public ActionResult Index()
        {
            var users = from User in _userRepository.GetUsers()
                        select User;
            return View(users);
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = _userRepository.GetUserByID(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View(new User());
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,UserPassword,UserType")] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _userRepository.InsertUsers(user);
                    _userRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                "Try again, and if the problem persists see your system administrator.");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = _userRepository.GetUserByID(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserName,UserPassword,UserType")] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _userRepository.UpdateUser(user);
                    _userRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, " +
                "and if the problem persists see your system administrator.");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = _userRepository.GetUserByID(id);

            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                User user = _userRepository.GetUserByID(id);
                _userRepository.DeleteUser(id);
                _userRepository.Save();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new System.Web.Routing.RouteValueDictionary {
                                    { "id", id },{ "saveChangesError", true } });
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userRepository.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Login/Out
        // GET: /Account/Login        
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            var userinfo = new LoginViewModel();

            try
            {
                // We do not want to use any existing identity information
                EnsureLoggedOut();

                // Store the originating URL so we can attach it to a form field
                userinfo.ReturnURL = returnUrl;

                return View(userinfo);
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel entity)
        {
            string OldHASHValue = string.Empty;
            byte[] SALT = new byte[saltLengthLimit];

            try
            {                
                // Ensure we have a valid viewModel to work with
                if (!ModelState.IsValid)
                    return View(entity);

                //Retrive Stored HASH Value From Database According To Username (one unique field)
                var userInfo = _userRepository.GetUserByID(entity.Username);

                //Assign HASH Value
                if (userInfo != null)
                    {
                        OldHASHValue = userInfo.HASH;
                        SALT = userInfo.SALT;

                    bool isLogin = CompareHashValue(entity.Password, entity.Username, OldHASHValue, SALT);

                    if (isLogin)
                    {
                        //Login Success
                        //For Set Authentication in Cookie (Remeber ME Option)
                        SignInRemember(entity.Username, entity.isRemember);

                        //Set A Unique ID in session
                        Session["UserID"] = userInfo.UserName;

                        // If we got this far, something failed, redisplay form
                        // return RedirectToAction("Index", "Dashboard");
                        return RedirectToLocal(entity.ReturnURL);
                    }
                    else
                    {
                        //Login Fail
                        TempData["ErrorMSG"] = "Access Denied! Wrong Credential";
                        return View(entity);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Login data is incorrect!");
                    return View(entity.ReturnURL);
                }

            }
            catch
            {
                throw;
            }
            //string OldHASHValue = string.Empty;
            //byte[] SALT = new byte[saltLengthLimit];

            //try
            //{                
            //        // Ensure we have a valid viewModel to work with
            //        if (!ModelState.IsValid)
            //    {
            //        //Retrive Stored HASH Value From Database According To Username (one unique field)
            //        var userInfo = _userRepository.GetUserByID(entity.Username);

            //        //Assign HASH Value
            //        if (userInfo != null)
            //        {
            //            OldHASHValue = userInfo.HASH;
            //            SALT = userInfo.SALT;
            //            bool isLogin = CompareHashValue(entity.Password, entity.Username, OldHASHValue, SALT);
            //            if (isLogin)
            //            {
            //                //Login Success
            //                //For Set Authentication in Cookie (Remeber ME Option)
            //                SignInRemember(entity.Username, entity.isRemember);

            //                //Set A Unique ID in session
            //                Session["UserID"] = userInfo.UserName;

            //                // If we got this far, something failed, redisplay form
            //                // return RedirectToAction("Index", "Dashboard");
            //                return RedirectToLocal(entity.ReturnURL);
            //            }
            //            else
            //            {
            //                //Login Fail
            //                TempData["ErrorMSG"] = "Access Denied! Wrong Credential";
            //                return View(entity);
            //            }

            //    }


            //    else
            //    {
            //        ModelState.AddModelError("", "Login data is incorrect!");
            //        return View(entity.ReturnURL);
            //    }  
            //}
            //catch
            //{
            //    throw;
            //}

        }
        

        

        //POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            try
            {
                // First we clean the authentication ticket like always
                //required NameSpace: using System.Web.Security;
                FormsAuthentication.SignOut();

                // Second we clear the principal to ensure the user does not retain any authentication
                //required NameSpace: using System.Security.Principal;
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

                Session.Clear();
                System.Web.HttpContext.Current.Session.RemoveAll();

                // Last we redirect to a controller/action that requires authentication to ensure a redirect takes place
                // this clears the Request.IsAuthenticated flag since this triggers a new request
                return RedirectToLocal();
            }
            catch
            {
                throw;
            }
        }        
        #endregion
        #region SaltandHash
        private static byte[] Get_SALT()
        {
            return Get_SALT(saltLengthLimit);
        }

        private static byte[] Get_SALT(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];

            //Require NameSpace: using System.Security.Cryptography;
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }

        public static string Get_HASH_SHA512(string password, string username, byte[] salt)
        {
            try
            {
                //required NameSpace: using System.Text;
                //Plain Text in Byte
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(password + username);

                //Plain Text + SALT Key in Byte
                byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + salt.Length];

                for (int i = 0; i < plainTextBytes.Length; i++)
                {
                    plainTextWithSaltBytes[i] = plainTextBytes[i];
                }

                for (int i = 0; i < salt.Length; i++)
                {
                    plainTextWithSaltBytes[plainTextBytes.Length + i] = salt[i];
                }

                HashAlgorithm hash = new SHA512Managed();
                byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);
                byte[] hashWithSaltBytes = new byte[hashBytes.Length + salt.Length];

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashWithSaltBytes[i] = hashBytes[i];
                }

                for (int i = 0; i < salt.Length; i++)
                {
                    hashWithSaltBytes[hashBytes.Length + i] = salt[i];
                }

                return Convert.ToBase64String(hashWithSaltBytes);
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion
        #region FunctLogin/Out
        public static bool CompareHashValue(string password, string username, string OldHASHValue, byte[] SALT)
        {
            try
            {
                string expectedHashString = Get_HASH_SHA512(password, username, SALT);

                return (OldHASHValue == expectedHashString);
            }
            catch
            {
                return false;
            }
        }


        //GET: EnsureLoggedOut
        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            if (Request.IsAuthenticated)
                Logout();
        }
        //GET: SignInAsync
        private void SignInRemember(string userName, bool isPersistent = false)
        {
            // Clear any lingering authencation data
            FormsAuthentication.SignOut();

            // Write the authentication cookie
            FormsAuthentication.SetAuthCookie(userName, isPersistent);
        }
        //GET: RedirectToLocal
        private ActionResult RedirectToLocal(string returnURL = "")
        {
            try
            {
                // If the return url starts with a slash "/" we assume it belongs to our site
                // so we will redirect to this "action"
                if (!string.IsNullOrWhiteSpace(returnURL) && Url.IsLocalUrl(returnURL))
                    return Redirect(returnURL);

                // If we cannot verify if the url is local to our host we redirect to a default location
                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                throw;
            }
        }
        #endregion

    }
}
