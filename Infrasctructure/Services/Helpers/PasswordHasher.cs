using Application.Models.Helpers;
using Application.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrasctructure.Services.Helpers
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordConfiguration _options;

        public PasswordHasher(IOptions<PasswordConfiguration> options)
        {
            _options = options.Value;
        }

        public bool Check(string hash, string password)
        {
            throw new NotImplementedException();
        }

        public string Hash(string password)
        {
            throw new NotImplementedException();
        }
    }
}
