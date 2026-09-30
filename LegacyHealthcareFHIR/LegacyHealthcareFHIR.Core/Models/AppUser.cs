using System;
using System.Collections.Generic;
using System.Text;

namespace LegacyHealthcareFHIR.Core.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public int HospitalId { get; set; }

        public Hospital Hospital { get; set; } = null!;
    }
}
