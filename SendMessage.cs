using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace SaveMoney
{
    class SendMessage
    {
        public void NotificationsForAdmin()
        {
            SerialPort portas = new SerialPort();
            portas = new SerialPort();
            portas.BaudRate = 9600;
            portas.PortName = "COM7";
            portas.DataBits = 8;
            portas.Open();
            portas.Write("1");
            portas.Close();

        }
    }
}
