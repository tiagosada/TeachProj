using System;
using Domain.Common;

namespace Domain.Students
{
    public interface IStudentsService : IService<Student>
    {
        CreatedEntityDTO Create(string name, string cpf, string phoneNumber, DateTime birthDate, string email, string registration);
        void Modify(Student student);
    }
}