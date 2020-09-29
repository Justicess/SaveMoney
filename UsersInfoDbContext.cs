using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SaveMoney
{
    public class UsersInfoDbContext : DbContext
    {
        // lenteles nustatomos
        public DbSet<UserData> userDatas { get; set; } // lenteles      
        public DbSet<UserSavings> usersavings { get; set; } //lenteles
        public DbSet<LogCheck> logChecks { get; set; } //lentele
    }
}
