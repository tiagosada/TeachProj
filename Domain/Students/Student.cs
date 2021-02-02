using System.Collections.Generic;
using Domain.Classrooms;
using Domain.Grades;
using Domain.Common;
using System;
using Domain.Users;
using Domain.Parents;

namespace Domain.Students
{
    public class Student : Person
    {
        public string Registration { get; set; }
        
        public virtual User User { get; set; }
        public Guid UserId { get; set; }

        public virtual Parent Parent { get; set; } = null;
        public Guid? ParentId { get; set; } = null;
        
        public virtual IList<ClassroomStudent> Classrooms { get; set; } = new List<ClassroomStudent>();
        public virtual IList<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();

        public Student(string name, string cpf, string phoneNumber, DateTime birthDate, string regist) : base(name, cpf, phoneNumber, birthDate)
        {
            Registration = regist;
        }

        protected Student() : base("", "", "", DateTime.MinValue) {}

        public (List<string> errors, bool isValid) Validate()
        {
            var errs = new List<string>(); 
            
            if (!ValidateName())
            {
                errs.Add("Invalid name");
            }
            if (!ValidateCPF())
            {
                errs.Add("Invalid CPF");
            }
            return (errs, errs.Count == 0);
        }

        public void LinkUser(User user)
        {
            User = user;
            UserId = user.Id;
        }
    }
}
