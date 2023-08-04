﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUp.Repository
{
    public class ClientRepository
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Email { get; }
        public string Password { get; }

        private ClientRepository(Guid Id, string Name, string Email, string Password)
        {
            this.Id = Id;
            this.Name = Name;
            this.Email = Email;
            this.Password = Password;
        }
    }
}
