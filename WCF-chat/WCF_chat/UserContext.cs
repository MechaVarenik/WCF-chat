using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace WCF_chat
{
   public class UserContext : DbContext
    {
        public UserContext() : base("UserContext")
        { }
        public DbSet<User> Users { get; set; }
    }
}
