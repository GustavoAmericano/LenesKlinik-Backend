using System;
using System.Collections.Generic;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices.Implementation
{
    public class BookingService : IBookingService
    {
        private IBookingRepository _repo;

        public BookingService(IBookingRepository repo)
        {
            _repo = repo;
        }

        public List<AvailableSessionsForDate> GetAvailableBookings(DateTime date, int duration)
        {

            if (date.Date < DateTime.Now.Date) throw new ArgumentException("Date was before today!");
            if (duration%15 != 0) throw new ArgumentException("Duration must be divisible by 15");

            var week = GetWeek(date);
            var startTime = new DateTime(1, 1, 1, 09, 0, 0); //might make this editable for the user
            var endTime = new DateTime(1, 1, 1, 17, 0, 0);
            List<AvailableSessionsForDate> availableBookingsArray = new List<AvailableSessionsForDate>();

            for (var i = 0; i < week.Length; i++)
            {
                List<AvailableSession> availableBookings = new List<AvailableSession>(); //This would contain start and end dates
                List<Booking> bookings = _repo.getBookingsByDate(week[i]); // SORT DATE, might pull this out to get all dates in a week
                
                var currentTime = new DateTime(week[i].Year , week[i].Month, week[i].Day, startTime.Hour, startTime.Minute, startTime.Second);

                if (!bookings.Count.Equals(0))
                {
                    var totalBookings = bookings.Count;
                    int count = 0;
                    while (count < totalBookings)
                    {
                        var currentBooking = bookings[count];
                        if (currentBooking.StartTime.TimeOfDay != currentTime.TimeOfDay)
                        {
                            while ((int)currentBooking.StartTime.TimeOfDay.Subtract(currentTime.TimeOfDay).TotalMinutes >= duration)
                            {
                                //This is a valid time
                                availableBookings.Add(new AvailableSession
                                {
                                    StartTime = currentTime,
                                    EndTime = currentTime.AddMinutes(duration)
                                });

                                currentTime = currentTime.AddMinutes(15);
                            }
                        }
                        currentTime = currentBooking.endTime;
                        count++;
                    }
                }

                while ((int)currentTime.TimeOfDay.TotalMinutes <= (int)endTime.TimeOfDay.TotalMinutes - duration)
                {
                    //valid time within work hours
                    availableBookings.Add(new AvailableSession
                    {
                        StartTime = currentTime,
                        EndTime = currentTime.AddMinutes(duration)
                    });

                    currentTime = currentTime.AddMinutes(15);
                }

                
                
                availableBookingsArray.Add(new AvailableSessionsForDate()
                {
                    AvailableSessions = availableBookings,
                    Date = week[i]
                });
            }

            return availableBookingsArray;
        }



        private DateTime[] GetWeek(DateTime date)
        {
            int numDay = 5;
            DateTime[] week = new DateTime[numDay];

            var monday = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            for (int i = 0; i < numDay; i++)
            {
                var weekDay = monday.AddDays(i);
                week[i] = weekDay;
            }

            return week;
        }

    }

}