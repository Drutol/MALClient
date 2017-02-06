using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using MALClient.Adapters;
using MALClient.UWP.Shared;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.Adapters
{
    public class CalendarExportProvider : ICalendarExportProvider
    {
        private static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public async void ExportToCalendar(object item)
        {
            var animeItemViewModel = item as AnimeItemViewModel;
            DayOfWeek day = Utilities.StringToDay(animeItemViewModel.TopLeftInfoBind);
            var date = GetNextWeekday(DateTime.Today, day);

            var timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            var startTime = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, timeZoneOffset);

            var appointment = new Appointment();

            appointment.StartTime = startTime;
            appointment.Subject = "Anime - " + animeItemViewModel.Title;
            appointment.AllDay = true;

            var recurrence = new AppointmentRecurrence();
            recurrence.Unit = AppointmentRecurrenceUnit.Weekly;
            recurrence.Interval = 1;
            recurrence.DaysOfWeek = UWPUtilities.DayToAppointementDay(day);
            if (animeItemViewModel.EndDate != AnimeItemViewModel.InvalidStartEndDate)
            {
                var endDate = DateTime.Parse(animeItemViewModel.EndDate);
                recurrence.Until = endDate;
            }
            else if (animeItemViewModel.StartDate != AnimeItemViewModel.InvalidStartEndDate &&
                     animeItemViewModel.AllEpisodes != 0)
            {
                var weeksPassed = (DateTime.Today - DateTime.Parse(animeItemViewModel.StartDate)).Days / 7;
                if (weeksPassed < 0)
                    return;
                var weeks = (uint)(animeItemViewModel.AllEpisodes - weeksPassed);
                recurrence.Until = DateTime.Today.Add(TimeSpan.FromDays(weeks * 7));
            }
            else if (animeItemViewModel.AllEpisodes != 0)
            {
                var epsLeft = animeItemViewModel.AllEpisodes - animeItemViewModel.MyEpisodes;
                recurrence.Until = DateTime.Today.Add(TimeSpan.FromDays(epsLeft * 7));
            }
            else
            {
                var msg = new MessageDialog("Not enough data to create event.");
                await msg.ShowAsync();
                return;
            }
            appointment.Recurrence = recurrence;
            var rect = new Rect(new Point(Window.Current.Bounds.Width / 2, Window.Current.Bounds.Height / 2), new Size());
            try
            {
                await AppointmentManager.ShowAddAppointmentAsync(appointment, rect, Placement.Default);
            }
            catch (Exception)
            {
                //appointpent is already being created
            }

        }
    }
}
