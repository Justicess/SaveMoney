using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveMoney
{
    public class UserSavings
    {

        // public int UserSavingsId { get; set; } // buvo komentas 

        // public int Savingsid { get; set; }

       [Key]
        public int id_Savings { get; set; }

        public double salary { get; set; }

        public double foodCosts { get; set; }

        public double fuelCosts { get; set; }

        public double tripsCosts { get; set; }

        public double gadgetsCosts { get; set; }

        public double savings { get; set; }
        //[Required] 1:1 relationship in entity
        // public int UserId { get; set; }

        //public int id { get; set; }
        

        //public virtual UserData UserData { get; set; }
    }
}
