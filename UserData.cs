using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace SaveMoney
{
 public class UserData
    {
        public int id { get; set; } 
        public string fakeNameLog { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool lockInfo { get; set; }
        public string number { get; set; }
        public string userPassword { get; set; }

       // public int UserId { get; set; }
     //  public virtual UserSavings UserSavings { get; set; }
    }
}
