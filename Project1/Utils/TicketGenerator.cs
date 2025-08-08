using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Utils
{
    static class TicketGenerator
    {
        private static int counter = 1;
        public static string GenerateTicket(string departmentCode)
        {
            string ticket = $"{departmentCode}-{counter:D3}";
            counter++;
            return ticket;
        }
    }
}
