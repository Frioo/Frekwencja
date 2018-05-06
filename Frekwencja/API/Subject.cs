namespace Frekwencja.API
{
    public class Subject
    {
        public string Id { get; }
        public string Name { get; }

        public Subject(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
