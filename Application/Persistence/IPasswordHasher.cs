﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistence
{
    public interface IPasswordHasher
    {
        string Hash(string password);

        bool Check(string hash, string password);
    }
}
