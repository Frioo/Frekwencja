using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Frekwencja.API
{
    public class SynergiaClient
    {
        private static string AuthUrl = @"https://api.librus.pl/OAuth/Token";
        private static string ApiUrl = @"https://api.librus.pl/2.0";
        private static string AuthToken = @"MzU6NjM2YWI0MThjY2JlODgyYjE5YTMzZjU3N2U5NGNiNGY=";
        private static readonly RestClient Client = new RestClient();
        private AccountData _currentUser;

        private List<Subject> Subjects = new List<Subject>();
        private List<Attendance> Attendances = new List<Attendance>();
        private List<Lesson> Lessons = new List<Lesson>();  

        public AccountData Login(string username, string password)
        {
            Client.BaseUrl = new Uri(AuthUrl);
            var loginRequest = new RestRequest(Method.POST);
            loginRequest.AddHeader("Authorization", $"Basic {AuthToken}");
            loginRequest.AddParameter("username", username);
            loginRequest.AddParameter("password", password);
            loginRequest.AddParameter("grant_type", "password");
            loginRequest.AddParameter("librus_long_term_token", "1");
            loginRequest.AddParameter("librus_rules_accepted", true);
            loginRequest.AddParameter("librus_mobile_rules_accepted", true);

            var loginResponse = Client.Execute(loginRequest);
            if (loginResponse.StatusCode == HttpStatusCode.OK)
            {
                Utils.Log("login attempt successful");
                var data = JObject.Parse(loginResponse.Content);
                var accessToken = data["access_token"].ToString();
                AccountData accountData = new AccountData(username, password, accessToken);
                _currentUser = accountData;
                PreloadData();
                return accountData;
            }
            Utils.Log($"login attempt failed: {loginResponse.StatusCode} -- {loginResponse.Content}");
            return null;
        }

        public async Task<List<Attendance>> GetAttendances()
        {
            var request = new RestRequest($"{ApiUrl}/Attendances", Method.GET);
            var data = (await FetchDataAsync(request))["Attendances"];
            var res = new List<Attendance>();
            for (int i = 0; i < data.Count; i++)
            {
                var attendanceObj = data[i];
                var id = attendanceObj["Id"].ToString();
                //var lesson = await GetLesson(attendanceObj["Lesson"]["Id"].ToString());
                var lesson = this.Lessons.Where(x => x.Id.Equals(attendanceObj["Lesson"]["Id"].ToString()))
                    .FirstOrDefault();
                var category = int.Parse(attendanceObj["Type"]["Id"].ToString());
                var result = new Attendance(id, lesson, category);
                res.Add(result);
            }

            return res;
        }

        public async Task<Lesson> GetLesson(string id)
        {
            var request = new RestRequest($"{ApiUrl}/Lessons/{id}", Method.GET);
            var data = (await FetchDataAsync(request))["Lesson"];
            var lId = data["Id"].ToString();
            //var lSubject = await GetSubject(data["Subject"]["Id"].ToString());
            var lSubject = this.Subjects.Where(x => x.Id.Equals(data["Subject"]["Id"].ToString()))
                .FirstOrDefault();
            var result = new Lesson(lId, lSubject);
            return result;
        }

        public async Task<List<Lesson>> GetLessons()
        {
            var request = new RestRequest($"{ApiUrl}/Lessons", Method.GET);
            var data = (await FetchDataAsync(request))["Lessons"];
            var res = new List<Lesson>();
            for (int i = 0; i < data.Count; i++)
            {
                var current = data[i];
                var id = current["Id"].ToString();
                var subject = this.Subjects.Where(x => x.Id.Equals(current["Subject"]["Id"].ToString()))
                    .FirstOrDefault();
                var currentRes = new Lesson(id, subject);
                res.Add(currentRes);
            }

            return res;
        }

        public async Task<Subject> GetSubject(string id)
        {
            var request = new RestRequest($"{ApiUrl}/Subjects/{id}", Method.GET);
            var data = (await FetchDataAsync(request))["Subject"];
            var subjectId = data["Id"].ToString();
            var subjectName = data["Name"].ToString();
            var result = new Subject(subjectId, subjectName);
            return result;
        }

        public async Task<List<Subject>> GetSubjects()
        {
            var request = new RestRequest($"{ApiUrl}/Subjects", Method.GET);
            var data = (await FetchDataAsync(request))["Subjects"];
            var res = new List<Subject>();
            for (int i = 0; i < data.Count; i++)
            {
                var current = data[i];
                var subjectId = current["Id"].ToString();
                var subjectName = current["Name"].ToString();
                var result = new Subject(subjectId, subjectName);
                res.Add(result);
            }

            return res;
        }

        public async void PreloadData()
        {
            this.Subjects = await GetSubjects();
            this.Lessons = await GetLessons();
            this.Attendances = await GetAttendances();
        }

        private async Task<dynamic> FetchDataAsync(IRestRequest request)
        {
            Utils.Log($"executing request: {request.Resource}");
            request.AddHeader("Authorization", $"Bearer {_currentUser.AccessToken}");
            var response = await Client.ExecuteTaskAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Utils.Log($"request successful: {request.Resource}");
                Utils.Log($"response contents: {response.Content}");
                return JObject.Parse(response.Content);
            }
            Utils.Log($"request failed: {request.Resource} ({response.StatusCode} -- {response.Content})");
            return null;
        }
    }
}
