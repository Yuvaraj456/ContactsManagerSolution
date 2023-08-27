using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsManager.Core.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Person Name Cannot be blank")]
        [StringLength(50)]
        
        public string PersonName { get; set; }

        [Required(ErrorMessage ="Email Cannot be blank")]
        [StringLength(50)]
        [EmailAddress(ErrorMessage ="Please enter valid email address")]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "IsEmailAlreadyRegistered", controller: "Account", ErrorMessage ="Email is already exist")] //async request to validate isalready exit email address
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number Cannot be blank")]      
        [Phone(ErrorMessage ="Please enter valid Phone Number")]
        [RegularExpression("^[0-9]*$",ErrorMessage ="Phone number should contain numbers only")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password Cannot be blank")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Confirm Password Name Cannot be blank")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public UsertypeOptions UserType { get; set; } = UsertypeOptions.User;
    }
}
