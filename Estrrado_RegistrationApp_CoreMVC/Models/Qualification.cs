using System.ComponentModel.DataAnnotations;

namespace Estrrado_RegistrationApp_CoreMVC.Models
{
    public class Qualification
    {
        [Key]
        public int Id { get; set; }

        public string StudentId { get; set; }

        public string CourseName { get; set; }
        public string University { get; set; }
        public int YearOfPassing { get; set; }
        public decimal Percentage { get; set; }
    }
}
