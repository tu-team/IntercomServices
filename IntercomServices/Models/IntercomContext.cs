﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace iLexStudio.IntercomServices.Models
{
    /// <summary>
    /// context of database
    /// </summary>
    public class UsersContext : DbContext
    {
     

        /// <summary>
        /// User profiles
        /// </summary>
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}