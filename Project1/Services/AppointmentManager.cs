using Project1.Models;
using Project1.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Services
{
    public class AppointmentManager
    {
        private University _university;
        public AppointmentManager(University university)
        {
            _university = university;
        }

        public void CreateAppointment()
        {
            try
            {
                ConsoleHelper.PrintLine("\n--- BOOK NEW APPOINTMENT ---", ConsoleColor.Green);

                Console.Write("Enter Student Name: ");
                string name = Console.ReadLine().Trim().ToUpper();

                Console.Write("Enter Student ID: ");
                string id = Console.ReadLine().Trim();

                ConsoleHelper.PrintLine("\nAvailable Services:", ConsoleColor.Cyan);
                for (int i = 0; i < _university.Services.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {_university.Services[i].ServiceName} ({_university.Services[i].DurationMinutes} min)");
                }

                Console.Write("Select a service: ");
                string input = Console.ReadLine();
                try
                {
                    int serviceChoice = int.Parse(input);
                    if (serviceChoice < 1 || serviceChoice > _university.Services.Count)
                    {
                        ConsoleHelper.PrintLine("Invalid service selection!", ConsoleColor.Red);
                        return;
                    }
                    Service selectedService = _university.Services[serviceChoice - 1];

                    Console.Write("Enter date (dd/MM/yyyy): ");
                    if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        ConsoleHelper.PrintLine("Invalid date format!", ConsoleColor.Red);
                        return;
                    }

                    // ✅ Prevent booking in the past
                    if (date < DateTime.Today)
                    {
                        ConsoleHelper.PrintLine("You cannot book an appointment in the past!", ConsoleColor.Red);
                        return;
                    }

                    // ✅ Check if same-day booking is still possible
                    if (date == DateTime.Today)
                    {
                        var lastSlot = _university.Appointments
                            .Where(a => a.AppointmentDate.Date == DateTime.Today &&
                                        a.BookedService.DepartmentCode == selectedService.DepartmentCode)
                            .OrderByDescending(a => a.AppointmentDate)
                            .FirstOrDefault();

                        // Office closes at 5PM
                        if (lastSlot != null && lastSlot.AppointmentDate.AddMinutes(selectedService.DurationMinutes) > DateTime.Today.AddHours(17))
                        {
                            ConsoleHelper.PrintLine("No more time slots available for today!", ConsoleColor.Red);
                            return;
                        }
                    }

                    DateTime appointmentTime = GetNextAvailableTime(date, selectedService);
                    string ticket = TicketGenerator.GenerateTicket(selectedService.DepartmentCode);

                    Appointment newApp = new Appointment(name, id, selectedService, appointmentTime, ticket);
                    _university.Appointments.Add(newApp);

                    ConsoleHelper.PrintLine("\n Appointment Booked Successfully!", ConsoleColor.Green);
                    ConsoleHelper.PrintLine($" Ticket: {ticket}", ConsoleColor.Yellow);
                    Console.WriteLine($" Date: {appointmentTime:dd/MM/yyyy}");
                    Console.WriteLine($" Time: {appointmentTime:HH:mm}");
                    Console.WriteLine($" Service: {selectedService.ServiceName}");
                }
                catch (FormatException)
                {
                    ConsoleHelper.PrintLine("Invalid service selection!", ConsoleColor.Red);
                    return;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintLine($"Error: {ex.Message}", ConsoleColor.Red);
            }
        }



        public void ViewAppointments()
        {
            ConsoleHelper.PrintLine("\n--- ALL APPOINTMENTS ---", ConsoleColor.Cyan);
            if (!_university.Appointments.Any())
            {
                ConsoleHelper.PrintLine("No appointments found!", ConsoleColor.Yellow);
                return;
            }

            var sortedApps = _university.Appointments.OrderBy(a => a.AppointmentDate);
            foreach (var app in sortedApps)
            {
                Console.WriteLine($"{app.TicketNumber} | {app.StudentName} | {app.StudentId} | {app.BookedService.ServiceName} | {app.AppointmentDate:dd/MM/yyyy HH:mm}");
            }
        }

        public void SearchAppointments()
        {
            Console.Write("Enter Student ID to search: ");
            string id = Console.ReadLine().Trim();

            var found = _university.Appointments.Where(a => a.StudentId == id);
            if (!found.Any())
            {
                ConsoleHelper.PrintLine("No appointments found for this ID.", ConsoleColor.Red);
                return;
            }

            ConsoleHelper.PrintLine("\n--- SEARCH RESULTS ---", ConsoleColor.Cyan);
            foreach (var app in found)
            {
                Console.WriteLine($"{app.TicketNumber} | {app.StudentName} | {app.BookedService.ServiceName} | {app.AppointmentDate:dd/MM/yyyy HH:mm}");
            }
        }

        public void UpdateAppointment()
        {
            Console.Write("Enter Ticket Number to reschedule: ");
            string ticket = Console.ReadLine().Trim().ToUpper();

            var app = _university.Appointments.FirstOrDefault(a => a.TicketNumber == ticket);
            if (app == null)
            {
                ConsoleHelper.PrintLine("Appointment not found!", ConsoleColor.Red);
                return;
            }

            Console.Write("Enter new date (dd/MM/yyyy): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate))
            {
                ConsoleHelper.PrintLine("Invalid date!", ConsoleColor.Red);
                return;
            }

            app.AppointmentDate = GetNextAvailableTime(newDate, app.BookedService);
            ConsoleHelper.PrintLine(" Appointment rescheduled successfully!", ConsoleColor.Green);
        }

        public void DeleteAppointment()
        {
            Console.Write("Enter Ticket Number to cancel: ");
            string ticket = Console.ReadLine().Trim().ToUpper();

            var app = _university.Appointments.FirstOrDefault(a => a.TicketNumber == ticket);
            if (app == null)
            {
                ConsoleHelper.PrintLine("Appointment not found!", ConsoleColor.Red);
                return;
            }

            _university.Appointments.Remove(app);
            ConsoleHelper.PrintLine("🗑 Appointment cancelled successfully!", ConsoleColor.Green);
        }

        private DateTime GetNextAvailableTime(DateTime date, Service service)
        {
            var sameDayApps = _university.Appointments
                .Where(a => a.AppointmentDate.Date == date.Date)
                .OrderBy(a => a.AppointmentDate)
                .ToList();

            DateTime start = date.Date.AddHours(9); // Office opens at 9 AM
            if (!sameDayApps.Any()) return start;

            DateTime lastTime = sameDayApps.Last().AppointmentDate.AddMinutes(service.DurationMinutes);
            return lastTime;
        }


    }
}
