namespace Frekwencja.API
{
    public class Attendance
    {
        public string Id { get; }
        public Lesson Lesson { get; set; }
        public int AttendanceCategory { get; }
        //public int AttendanceCategoryId { get; }

        public Attendance(string id, Lesson lesson, int attendanceCategory/*, int attendanceCategoryId*/)
        {
            Id = id;
            Lesson = lesson;
            AttendanceCategory = attendanceCategory;
            //AttendanceCategoryId = attendanceCategoryId;
        }
    }
}
