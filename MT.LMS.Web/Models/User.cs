using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace MT.LMS.Web.Models
{
    public class User
    {
        [Key]
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }


        [Required]
        [RegularExpression("^((?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\W])).+$", ErrorMessage = "Passwords must have at least one non letter. Passwords must have at least one digit ('0'-'9'). Passwords must have at least one uppercase and one lowercase('A'-'Z').")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayName("User Password")]
        public string UserPassword { get; set; }


        [Required(ErrorMessage = "User Type is a required field")]
        [DisplayName("User Type")]
        public string UserType { get; set; }

        public byte[] SALT { get; set; }
        public string HASH { get; set; }
    }

    public class RegisterUserViewModel
    {        
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }


        [Required]
        [RegularExpression("^((?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\W])).+$", ErrorMessage = "Passwords must have at least one non letter. Passwords must have at least one digit ('0'-'9'). Passwords must have at least one uppercase and one lowercase('A'-'Z').")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayName("User Password")]
        public string UserPassword { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("UserPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "User Type is a required field")]
        [DisplayName("User Type")]
        public string UserType { get; set; }
    }

    public class UserAccount
    {
        public string UserHash { get; set; }
        public string UserSalt { get; set; }
        public string UserType { get; set; }       

    }
}