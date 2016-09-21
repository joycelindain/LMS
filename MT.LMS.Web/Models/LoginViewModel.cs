using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MT.LMS.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User Name is a required field")]
        [DisplayName("Username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "User Password is a required field")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        public string ReturnURL { get; set; }

        public bool isRemember { get; set; }

    }

    
}