using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Estrrado_RegistrationApp_CoreMVC.Models
{
    public class Student
    {
        [Key]
        public string StudentId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required, EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        // Initialize collection to avoid nulls
        public ICollection<Qualification> Qualifications { get; set; } = new List<Qualification>();
    }
}
