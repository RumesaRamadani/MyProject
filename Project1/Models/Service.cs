namespace Project1.Models
{
    public class Service
    {
        public string ServiceName { get; set; }
        public int DurationMinutes { get; set; }
        public string DepartmentCode { get; set; }

        public Service(string serviceName, int durationMinutes, string departmentCode)
        {
            ServiceName = serviceName;
            DurationMinutes = durationMinutes;
            DepartmentCode = departmentCode;
        }

        public override string ToString()
        {
            return $"[{DepartmentCode}] {ServiceName} - Duration: {DurationMinutes} minutes";
        }
    }
}
