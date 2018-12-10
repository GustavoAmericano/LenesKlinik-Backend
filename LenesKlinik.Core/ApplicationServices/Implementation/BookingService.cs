using System;
using System.Collections.Generic;
using System.Linq;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices.Implementation
{
    public class BookingService : IBookingService
    {
        private IBookingRepository _repo;
        private IWorkRepository _workRepo;

        public BookingService(IBookingRepository repo, IWorkRepository workRepo)
        {
            _repo = repo;
            _workRepo = workRepo;
        }

        public List<AvailableSessionsForDate> GetAvailableBookings(DateTime date, int workId)
        {

            Work work = _workRepo.GetWorkById(workId);

            var duration = work.Duration;
            
            if (date.Date < DateTime.Now.Date) throw new ArgumentException("Date was before today!");
            if (duration%15 != 0) throw new ArgumentException("Duration must be divisible by 15");

            var week = GetWeek(date);
            var startTime = new DateTime(1, 1, 1, 09, 0, 0); //might make this editable for the user
            var endTime = new DateTime(1, 1, 1, 17, 0, 0);
            List<AvailableSessionsForDate> availableBookingsArray = new List<AvailableSessionsForDate>();

            for (var i = 0; i < week.Length; i++)
            {
                List<AvailableSession> availableBookings = new List<AvailableSession>(); //This would contain start and end dates
                List<Booking> bookings = _repo.GetBookingsByDate(week[i]).OrderBy(book => book.StartTime).ToList();
                
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
                        currentTime = currentBooking.EndTime;
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

        public Booking SaveBooking(Booking booking)
        {
            if(booking.StartTime.Minute % 15 != 0) throw new ArgumentException("Invalid start time!");
            if(booking.EndTime.Minute % 15 != 0) throw new ArgumentException("Invalid end time!");
            if (booking.EndTime.Subtract(booking.StartTime).TotalMinutes < 0)
                throw new ArgumentException("Invalid time - End before start!");
            return _repo.SaveBooking(booking);
        }

        public List<BookingInfo> GetBookingsForWeek(DateTime date)
        {
            DateTime[] week = GetWeek(date);
            List<BookingInfo> bookings = new List<BookingInfo>();
            foreach (var day in week)
            {
                bookings.Add(new BookingInfo
                {
                    Date = day,
                    Bookings = _repo.GetBookingsByDate(day)
                });
            }
            return bookings;
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