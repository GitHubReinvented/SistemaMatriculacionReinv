﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ReinventedPuembo.Models
{
    public class LoginViewModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool Recuerdame { get; set; } = true;
    }
}
