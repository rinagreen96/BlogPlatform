using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForBlogs.Models
{
    public class ClientProfileModel
    {
        public string ID { get; set; }

        [Required(ErrorMessage = "Email is a requiered field!")]
        [RegularExpression(@"([^а-яА-Я]*)", ErrorMessage = "Format of the Email is not correct")]
        [EmailAddress(ErrorMessage = "Format of the Email is not correct")]
        [StringLength(30, ErrorMessage = "Length of the Email should not exceed 30 characters")]
        public string Email { get; set; }
        public string Password { get; set; }

        [StringLength(50, ErrorMessage = "Length of the Surname should not exceed 50 characters")]
        [RegularExpression(@"([A-Za-z-]*)", ErrorMessage = "The Surname can contains only latin letters and a sign '-'")]
        [Display(Name = "Surname")]
        public string Sername { get; set; }

        [Required(ErrorMessage = "Name is a requiered field!")]
        [StringLength(50, ErrorMessage = "Length of the Name should not exceed 50 characters")]
        [RegularExpression(@"([A-Za-z-]*)", ErrorMessage = "The Name can contains only latin letters and a sign '-'")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Length of the NickName should not exceed 100 characters")]
        [RegularExpression(@"([A-Za-z0-9-_]*)", ErrorMessage = "The NickName can contains only latin letters, numbers and signs '-' '_'")]
        public string Nickname { get; set; }

        public DateTime Date_of_registration { get; set; }

        [StringLength(50, ErrorMessage = "Length of the City should not exceed 50 characters")]
        [RegularExpression(@"([A-Za-z-]*)", ErrorMessage = "The City can contains only latin letters and a sign '-'")]
        public string City { get; set; }

        public string Avatar_path { get; set; }

        public string Info { get; set; }
    }
}