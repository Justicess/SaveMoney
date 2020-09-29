using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveMoney
{
    class User
    {
        public int User_ID { get; set; }
        public string logname { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string UserPassword { get; set; }
        public bool LockStatus { get; set; }
        public string User_Phone { get; set; }
    }
}
