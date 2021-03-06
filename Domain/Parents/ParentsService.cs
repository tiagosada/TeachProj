using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.MailServices;
using Domain.MailServices.Templates;
using Domain.Students;
using Domain.Users;

namespace Domain.Parents
{
    public class ParentsService : Service<Parent>, IParentsService
    {
        private readonly IParentsRepository _parentsRepository;
        private readonly IStudentsService _studentsService;
        private readonly IUsersService _usersService;
        
        public ParentsService(IParentsRepository parentsRepository, IUsersService usersService, IStudentsService studentsService) : base(parentsRepository)
        {
            _parentsRepository = parentsRepository;
            _usersService = usersService;
            _studentsService = studentsService;
        }

        public CreatedEntityDTO Create(string name, string cpf, string phoneNumber, DateTime birthDate, string email, string registration)
        {
            if (_parentsRepository.Get(x => x.CPF == cpf) != null)
            {
                return new CreatedEntityDTO(new List<string>{"Parent already exists"});
            }

            if (_parentsRepository.Get(x => x.Email == email) != null)
            {
                return new CreatedEntityDTO(new List<string>{"Email already in use"});
            }

            var student = _studentsService.Get(x => x.Registration == registration);
            if (student == null)
            {
                return new CreatedEntityDTO(new List<string>{"Student not found"});
            }

            if (student.ParentId != null)
            {
                return new CreatedEntityDTO(new List<string>{"Student already has parent"});
            }
            
            var parent = new Parent(name, cpf, phoneNumber, birthDate, email, student);
            
            var parentVal = parent.Validate();
            if (!parentVal.isValid)
            {
                return new CreatedEntityDTO(parentVal.errors);
            }
            
            var userCreated = _usersService.Create(Profile.Parent, cpf, birthDate.ToString("ddMMyyyy"));
            if (!userCreated.IsValid)
            {
                return new CreatedEntityDTO(userCreated.Errors);
            }

            if (!String.IsNullOrWhiteSpace(email))
            {
                var mailservice = new MailService();
                mailservice.Send(TemplateType.ParentRegistration, parent);
            }

            var user = _usersService.Get(userCreated.Id);
            parent.LinkUser(user);
            _parentsRepository.Add(parent);
            return new CreatedEntityDTO(parent.Id);
        }

        public override bool Remove(Guid id)
        {
            var parent = _parentsRepository.Get(x => x.Id == id);
            if (parent == null) { return false; }

            var user = _usersService.Get(parent.UserId);
            if (user != null)
            {
                _usersService.Remove(user.Id);
            }

            if (_parentsRepository.Get(x => x.Id == parent.Id) != null)
            {
                _parentsRepository.Remove(parent);
            }
            
            return true;
        }
    }
}