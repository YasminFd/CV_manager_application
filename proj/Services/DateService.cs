namespace proj.Services
{
    public class DateService
    {
        public static bool checkIfPastDate(DateTime? date)
        {
            return date > DateTime.Today;
        }
        public static bool checkMinimumAge(DateTime? date)
        {
            DateTime minimumBirthDate = DateTime.Today.AddYears(-14);
            return date > minimumBirthDate;
        }
    }
}
