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

        public List<DateSessions> GetAvailableBookings(DateTime date, int workId)
        {

            
            Work work = _workRepo.GetWorkById(workId); // Get the work, the workId points to
            var duration = work.Duration;  // Get the duration of the Work

            //If before today, we do not wish to look for available sessions.
            if (date.Date < DateTime.Now.Date) throw new ArgumentException("Date was before today!");
            //If not divisible by 15, it's an invalid duration.
            if (duration%15 != 0) throw new ArgumentException("Duration must be divisible by 15");
            //Fetch mon - fri for the week that contains the input date.
            var week = GetWeek(date);
            // HARDCODED: Daily time that the customer starts her day.
            var startTime = new DateTime(1, 1, 1, 09, 0, 0); //might make this editable for the user
            // HARDCODED: Daily time that the customer ends her day.
            var endTime = new DateTime(1, 1, 1, 17, 0, 0);
            //A list that will contain DateSessions, which is an entity containing a date,
            //and a list of available sessions for that date.
            List<DateSessions> DateSessionsList = new List<DateSessions>();

            // For each date in the week:
            for (var i = 0; i < week.Length; i++)
            {
                // List that will available sessions
                List<AvailableSession> availableBookings = new List<AvailableSession>();

                //If date is before today, ignore
                if (week[i].Date >= DateTime.Today)
                {
                    // Fetch currently booked sessions for the day
                    List<Booking> bookings = _repo.GetBookingsByDate(week[i]).OrderBy(book => book.StartTime).ToList();

                    // Set start time to the dates date, and the start time of customers workdate
                    var currentTime = new DateTime(week[i].Year, week[i].Month, week[i].Day, startTime.Hour, startTime.Minute, startTime.Second);

                    // If there was previously booked sessions:
                    if (!bookings.Count.Equals(0))
                    {
                        var totalBookings = bookings.Count; // How many bookings are there
                        int count = 0; // Start at 0 (0-indexed)
                        while (count < totalBookings) // While there's still booked sessions:
                        {
                            var currentBooking = bookings[count]; // current booking we work with
                            if (currentBooking.StartTime.TimeOfDay != currentTime.TimeOfDay) // If currentTime and booked sessions time ain't equal:
                            {
                                // While the time between our current time and the booked sessions startTime is 
                                // bigger or equal to the duration of the Work:
                                while ((int)currentBooking.StartTime.TimeOfDay.Subtract(currentTime.TimeOfDay).TotalMinutes >= duration)
                                {
                                    //Add the sessions to the list
                                    availableBookings.Add(new AvailableSession
                                    {
                                        StartTime = currentTime,
                                        EndTime = currentTime.AddMinutes(duration)
                                    });

                                    // Add 15 minutes to the currentTime and loop.
                                    currentTime = currentTime.AddMinutes(15);
                                }
                            }
                            // When there's no longer enough time between booking and current time, go to endTime of the booking and loop.
                            currentTime = currentBooking.EndTime;
                            count++;
                        }
                    }

                    // If no previously booked sessions remains:
                    // While the time between the currentTime and the end of customers workday:
                    while ((int)currentTime.TimeOfDay.TotalMinutes <= (int)endTime.TimeOfDay.TotalMinutes - duration)
                    {
                        //Add the available session to the list
                        availableBookings.Add(new AvailableSession
                        {
                            StartTime = currentTime,
                            EndTime = currentTime.AddMinutes(duration)
                        });

                        // Add 15minutes and loop
                        currentTime = currentTime.AddMinutes(15);
                    }

                }
                
                    // Once we're done finding all sessions for the day, add them to the DateSession entity, and give that the
                    // The current date aswell. Add to the list of DateSessions.
                    DateSessionsList.Add(new DateSessions()
                    {
                        AvailableSessions = availableBookings,
                        Date = week[i]
                    });
            }

            // Return list of DateSessions
            return DateSessionsList;
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

        public List<Booking> GetBookingsByCustomerId(int customerId)
        {
            try
            {
                List<Booking> bookings =  _repo.GetBookingsByCustomerId(customerId);
                if (bookings.Count <= 0) return null;
                return bookings;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteBooking(int bookingId)
        {
            try
            {
                _repo.DeleteBooking(bookingId);
            }
            catch (Exception e)
            {
                throw e;
            }
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