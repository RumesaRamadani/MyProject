using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class University
    {
        public string Name { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<Service> Services { get; set; }

        public University(string name)
        {
            Name = name;
            Appointments = new List<Appointment>();
            Services = new List<Service>();
        }
    }
}
