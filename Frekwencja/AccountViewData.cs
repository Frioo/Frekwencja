namespace Frekwencja
{
    public class AccountViewData
    {
        public string Identifier { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName { get; }

        public AccountViewData(string id, string firstName, string lastName)
        {
            Identifier = id;
            FirstName = firstName;
            LastName = lastName;
            FullName = $"{FirstName} {LastName}";
        }
    }
}
