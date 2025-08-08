using System;

namespace Project1.Models
{
    public class Appointment
    {
        public string StudentName { get; set; }
        public string StudentId { get; set; }
        public Service BookedService { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string TicketNumber { get; set; }
        public Appointment(string studentName, string studentId, Service bookedService, DateTime appointmentDate, string ticketNumber)
        {
            StudentName = studentName;
            StudentId = studentId;
            BookedService = bookedService;
            AppointmentDate = appointmentDate;
            TicketNumber = ticketNumber;
        }
        public override string ToString()
        {
            return $"{TicketNumber} | {StudentName} ({StudentId}) | {BookedService.ServiceName} | Date: {AppointmentDate:dd/MM/yyyy HH:mm}";
        }

    }
}