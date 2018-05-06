using System;
using System.Collections.Generic;
using Frekwencja.API;

namespace Frekwencja
{
    public class AttendanceInfo
    {
        public bool DataAvailable { get; }
        public int LessonCount { get; }
        public int Presences { get; }
        public int Absences { get; }
        public int Late { get; }
        public double PresenceRatio { get; }

        public AttendanceInfo(List<Attendance> attendances)
        {
            this.DataAvailable = true;
            this.LessonCount = attendances.Count;

            if (LessonCount <= 0)
            {
                this.DataAvailable = false;
                return;
            }

            for (int i = 0; i < attendances.Count; i++)
            {
                if (attendances[i].AttendanceCategory == 100)
                    Presences++;
                else if (attendances[i].AttendanceCategory == 2)
                    Late++;
                else
                    Absences++;
            }
            Utils.Log($"presences: {Presences}; absences {Absences}; late: {Late}");
            this.PresenceRatio = ((double) Presences / (double) LessonCount) * 100;
            this.PresenceRatio = Math.Round(PresenceRatio, 1);
        }
    }
}
