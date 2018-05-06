namespace Frekwencja.API
{
    public class Lesson
    {
        public string Id { get; }
        public Subject Subject { get; }

        public Lesson(string id, Subject subject)
        {
            Id = id;
            Subject = subject;
        }
    }
}
