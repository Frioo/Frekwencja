using System.Windows.Controls;
using Frekwencja.API;

namespace Frekwencja
{
    public class SubjecListItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public Subject ValueObject { get; set; }

        public SubjecListItem(Subject subject)
        {
            Text = subject.Name;
            Value = subject.Id;
            ValueObject = subject;
        }
    }
}
