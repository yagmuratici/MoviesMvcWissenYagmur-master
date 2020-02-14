using _036_MoviesMvcWissen.Validations.FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _036_MoviesMvcWissen.Entities
{
    [Validator(typeof(UserValidator))] //user validator un içindekileri burada active hale getiriyoruz.
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(25)]
        public string Password { get; set; }
    }
}