namespace Frekwencja
{
    public class AttendanceProperty
    {
        public string Description { get; set; }
        public string Value { get; set; }

        public AttendanceProperty(string description, string value)
        {
            this.Description = description;
            this.Value = value;
        }
    }
}
