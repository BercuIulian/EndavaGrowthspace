﻿using Microsoft.AspNetCore.Identity;

namespace EndavaGrowthspace.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty ;
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
