using System.Diagnostics;

namespace Frekwencja
{
    public static class Utils
    {
        public static void Log(string text)
        {
            Debug.WriteLine($"Kalkulator frewkencji (log) --> {text}");
        }
    }

    public enum AttendanceCategory
    {
        Absent = 1,
        Late = 2,
        Justified = 3,
        Excused = 4,
        Present = 100
    }
}
