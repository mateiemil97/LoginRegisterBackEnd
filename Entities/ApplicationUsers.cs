﻿using System;
using Microsoft.AspNetCore.Identity;

namespace Entities
{
    public class ApplicationUser: IdentityUser
    {
        public int EmpId { get; set; }
    }
}
