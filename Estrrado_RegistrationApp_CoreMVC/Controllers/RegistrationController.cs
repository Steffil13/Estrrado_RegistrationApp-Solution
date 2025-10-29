using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Estrrado_RegistrationApp_CoreMVC.Data;
using Estrrado_RegistrationApp_CoreMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Estrrado_RegistrationApp_CoreMVC.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly AppDbContext _context;

        public RegistrationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Registration
        public IActionResult Index()
        {
            var students = _context.Students
                                   .Include(s => s.Qualifications)
                                   .OrderBy(s => s.StudentId)
                                   .ToList();
            return View(students);
        }

        // GET: Registration/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Registration/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            // server-side validation
            //if (!ModelState.IsValid)
                //return View(student);

            // Generate StudentId
            var newId = GenerateStudentId();

            var studentToAdd = new Student
            {
                StudentId = newId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Age = student.Age,           
                DOB = student.DOB,
                Gender = student.Gender,
                Email = student.Email,
                Phone = student.Phone,
                Username = string.IsNullOrWhiteSpace(student.Username) ? newId.ToLower() : student.Username,
                Password = student.Password 
            };

            _context.Students.Add(studentToAdd);
            _context.SaveChanges(); 

            // === Gather qualifications ===
            var qualsToAdd = new List<Qualification>();

            // 1) If model binder supplied qualifications, use them
            if (student.Qualifications != null && student.Qualifications.Any())
            {
                foreach (var q in student.Qualifications)
                {
                    if (!IsQualificationEmpty(q))
                    {
                        qualsToAdd.Add(new Qualification
                        {
                            StudentId = newId,
                            CourseName = q.CourseName,
                            University = q.University,
                            YearOfPassing = q.YearOfPassing,
                            Percentage = q.Percentage
                        });
                    }
                }
            }

            
            if (!qualsToAdd.Any())
            {
                for (int i = 0; i < 50; i++) // upper bound for rows
                {
                    var courseKey = $"Qualifications[{i}].CourseName";
                    var uniKey = $"Qualifications[{i}].University";
                    var yearKey = $"Qualifications[{i}].YearOfPassing";
                    var percKey = $"Qualifications[{i}].Percentage";

                    if (!Request.Form.ContainsKey(courseKey) && !Request.Form.ContainsKey(uniKey)
                        && !Request.Form.ContainsKey(yearKey) && !Request.Form.ContainsKey(percKey))
                    {
                       
                        continue;
                    }

                    var course = Request.Form[courseKey].FirstOrDefault();
                    var uni = Request.Form[uniKey].FirstOrDefault();
                    var yearStr = Request.Form[yearKey].FirstOrDefault();
                    var percStr = Request.Form[percKey].FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(course) && string.IsNullOrWhiteSpace(uni)
                        && string.IsNullOrWhiteSpace(yearStr) && string.IsNullOrWhiteSpace(percStr))
                    {
                        continue;
                    }

                    int.TryParse(yearStr, out int year);
                    decimal.TryParse(percStr, out decimal perc);

                    qualsToAdd.Add(new Qualification
                    {
                        StudentId = newId,
                        CourseName = course,
                        University = uni,
                        YearOfPassing = year,
                        Percentage = perc
                    });
                }
            }

            if (qualsToAdd.Any())
            {
                _context.Qualifications.AddRange(qualsToAdd);
                _context.SaveChanges();
            }

            // Redirect to Login view (GET) 
            return RedirectToAction("Index", "Login");
        }

        // GET: Registration/Details/{id}
        public IActionResult Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var student = _context.Students
                                  .Include(s => s.Qualifications)
                                  .FirstOrDefault(s => s.StudentId == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        // GET: Registration/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Helper: generate StudentId like STD001, STD002
        private string GenerateStudentId()
        {
            var last = _context.Students
                               .OrderByDescending(s => s.StudentId)
                               .Select(s => s.StudentId)
                               .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(last))
                return "STD001";

            var numericPart = new string(last.Where(char.IsDigit).ToArray());
            if (!int.TryParse(numericPart, out int num))
            {
                int count = _context.Students.Count() + 1;
                return "STD" + count.ToString("000");
            }

            return "STD" + (num + 1).ToString("000");
        }

        // Helper: detect empty qualification row
        private bool IsQualificationEmpty(Qualification q)
        {
            if (q == null) return true;
            return string.IsNullOrWhiteSpace(q.CourseName)
                && string.IsNullOrWhiteSpace(q.University)
                && (q.YearOfPassing == 0)
                && (q.Percentage == 0);
        }
    }
}
