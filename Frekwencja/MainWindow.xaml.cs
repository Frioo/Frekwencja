using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Frekwencja.API;

namespace Frekwencja
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static SynergiaClient Client = new SynergiaClient();
        
        public ObservableCollection<AccountData> AccountDatum = new ObservableCollection<AccountData>();
        public List<Attendance> Attendances;
        public string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public MainWindow()
        {
            InitializeComponent();
            this.Title = $"Frekwencja {Version}";
            ListBoxSubjects.DisplayMemberPath = "Text";
            ListBoxSubjects.SelectedValuePath = "Value";
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = false;
            LoginButton.Content = "Logowanie...";
            var identifier = IdentifierBox.Text;
            var password = PasswordBox.Password;

            var data = await Task.Factory.StartNew(() => Client.Login(identifier, password));

            if (data == null)
            {
                MessageBox.Show("Wpisano nieprawdiłowe dane logowania lub wystąpił problem z połączeniem.", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                LoginButton.Content = "Pobieranie danych...";
                this.Title = $"Frekwencja {Version} | {identifier}";
                ListBoxSubjects.Items.Clear();
                await ProcessAttendances();
            }

            GlobalStatsButton.IsEnabled = true;
            LoginButton.IsEnabled = true;
            LoginButton.Content = "Zaloguj się";
        }

        private async Task ProcessAttendances()
        {
            Attendances = await Client.GetAttendances();
            var subjects = await Client.GetSubjects();
            Utils.Log($"subjects: {subjects.Count}");
            for (int i = 0; i < subjects.Count; i++)
            {
                ListBoxSubjects.Items.Add(new SubjecListItem(subjects[i]));
            }
        }

        private void ListBoxSubjects_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewAttendanceData.Items.Clear();
            ListViewAttendanceDataHeader.Content = "Frekwencja (dane dla wybranego przedmiotu)";
            if (ListBoxSubjects.SelectedValue == null) return;
            Utils.Log($"Selected value: {ListBoxSubjects.SelectedValue}");
            var attendancesForSubject = new List<Attendance>();
            Utils.Log($"Attendances: {Attendances.Count}");
            for (int i = 0; i < Attendances.Count; i++)
            {
                if (Attendances[i].Lesson.Subject.Id.Equals(ListBoxSubjects.SelectedValue.ToString()))
                {
                    attendancesForSubject.Add(Attendances[i]);
                }
            }

            AddListItems(new AttendanceInfo(attendancesForSubject));
        }

        private void GlobalStatsButton_OnClick(object sender, RoutedEventArgs e)
        {
            Utils.Log("global stats button: click");
            ListViewAttendanceData.Items.Clear();
            ListViewAttendanceDataHeader.Content = "Frekwencja (dane ze wszystkich przedmiotów)";

            AddListItems(new AttendanceInfo(Attendances));
        }

        private void AddListItems(AttendanceInfo info)
        {
            if (!info.DataAvailable)
            {
                ListViewAttendanceData.Items.Add("Brak danych");
                return;
            }

            ListViewAttendanceData.Items.Add($"Liczba lekcji: {info.LessonCount}");
            ListViewAttendanceData.Items.Add($"Obecności: {info.Presences}");
            ListViewAttendanceData.Items.Add($"Nieobecności: {info.Absences}");
            ListViewAttendanceData.Items.Add($"Spóźnienia: {info.Late}");
            ListViewAttendanceData.Items.Add($"Frekwencja: {info.PresenceRatio}%");
        }
    }
}
