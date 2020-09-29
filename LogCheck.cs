using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveMoney
{
   public class LogCheck
   {
        [Key]
        public int LogData_id { get; set; }
        public int logData { get; set; }
        //public virtual UserData UserData { get; set; }
    }
}
