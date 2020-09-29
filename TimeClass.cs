using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveMoney
{
    class TimeClass
    {
        public virtual int GetTodaysDay()
        {
            DateTime MonthdateTime = DateTime.Today;
            int monthReturn = MonthdateTime.Month;
            return monthReturn;
        }
        public virtual int GetYear()
        {
            DateTime currentYears = DateTime.Today;
            int curretntYear = currentYears.Year;
            return curretntYear;
        }
        
    }
   
}
