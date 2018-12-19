using System;
using System.Collections.Generic;
using System.Linq;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices.Implementation
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly IWorkRepository _workRepo;
        private readonly IEmailService _emailService;

        public BookingService(IBookingRepository repo, IWorkRepository workRepo, IEmailService emailService)
        {
            _emailService = emailService;
            _repo = repo;
            _workRepo = workRepo;
        }

        public List<DateSessions> GetAvailableBookings(DateTime date, int workId)
        {
            Work work = _workRepo.GetWorkById(workId); 
            var duration = work.Duration;  

            if (date.Date < DateTime.Now.Date) throw new ArgumentException("Date was before today!");
            var week = GetWeek(date);
            var startTime = new DateTime(1, 1, 1, 09, 0, 0); 
            var endTime = new DateTime(1, 1, 1, 17, 0, 0);
            List<DateSessions> DateSessionsList = new List<DateSessions>();

            for (var i = 0; i < week.Length; i++)
            {
                List<AvailableSession> availableSessions = new List<AvailableSession>();

                if (week[i].Date >= DateTime.Today)
                {
                    List<Booking> bookings = _repo.GetBookingsByDate(week[i])
                        .OrderBy(book => book.StartTime).ToList();

                    var currentTime = new DateTime(week[i].Year, week[i].Month, week[i].Day,
                        startTime.Hour, startTime.Minute, startTime.Second);

                    if (!bookings.Count.Equals(0))
                    {
                        for (int j = 0; j < bookings.Count; j++)
                        {
                            var currentBooking = bookings[j];
                            if (currentBooking.StartTime.TimeOfDay != currentTime.TimeOfDay)
                            {
                                while ((int)currentBooking.StartTime.TimeOfDay
                                           .Subtract(currentTime.TimeOfDay).TotalMinutes >= duration)
                                {
                                    availableSessions.Add(new AvailableSession
                                    {
                                        StartTime = currentTime,
                                        EndTime = currentTime.AddMinutes(duration)
                                    });
                                    currentTime = currentTime.AddMinutes(15);
                                }
                            }
                            currentTime = currentBooking.EndTime;
                        }
                    }

                    while ((int)currentTime.TimeOfDay.TotalMinutes <= (int)endTime.TimeOfDay.TotalMinutes - duration)
                    {
                        availableSessions.Add(new AvailableSession
                        {
                            StartTime = currentTime,
                            EndTime = currentTime.AddMinutes(duration)
                        });

                        currentTime = currentTime.AddMinutes(15);
                    }

                }
                    DateSessionsList.Add(new DateSessions()
                    {
                        AvailableSessions = availableSessions,
                        Date = week[i]
                    });
            }
            return DateSessionsList;
        }

        public Booking SaveBooking(Booking booking)
        {
            if(booking.StartTime.Minute % 15 != 0)
                throw new ArgumentException("Invalid start time!");
            if(booking.EndTime.Minute % 15 != 0)
                throw new ArgumentException("Invalid end time!");
            if (booking.EndTime.Subtract(booking.StartTime).TotalMinutes < 0)
                throw new ArgumentException("Invalid time - End before start!");

            try
            {
                return _repo.SaveBooking(booking);
            }
            catch (Exception)
            {
                throw new Exception("An Error occured trying to save the booking.");
            }
        }

        public List<BookingInfo> GetBookingsForWeek(DateTime date)
        {
            DateTime[] week = GetWeek(date);
            List<BookingInfo> bookings = new List<BookingInfo>();
            try
            {
                foreach (var day in week)
                {
                    bookings.Add(new BookingInfo
                    {
                        Date = day,
                        Bookings = _repo.GetBookingsByDate(day)
                    });
                }
            }
            catch (Exception)
            {
                throw new Exception("An Error occured trying to fetch the bookings.");
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
                throw new Exception("An Error occured fetch to save the bookings.");
            }
        }

        public void DeleteBooking(int bookingId)
        {
            try
            {
               Booking booking =  _repo.DeleteBooking(bookingId);
                if (booking == null) throw new ArgumentException("No booking found with specified ID!");
                SendCancelMail(booking);
            }
            catch (Exception)
            {
                throw new Exception("An Error occured trying to delete the booking.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="booking"></param>
        private void SendCancelMail(Booking booking)
        {
            try
            {
                _emailService.SendMail("makeklinik@gmail.com", "Booking aflyst!", 
                    $"Booking d. {booking.StartTime.Date} {booking.StartTime.TimeOfDay} - {booking.EndTime.TimeOfDay} er blevet aflyst.");
                _emailService.SendMail(booking.Customer.User.Email, "Booking aflyst!", GenerateEmailBody(booking));
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to send mail!");
            }
        }

        private string GenerateEmailBody(Booking booking)
        {
            return $"Hej, {booking.Customer.Firstname}." +
                   $"\nDin tid hos Lenes Klinik d. {booking.StartTime.Date} {booking.StartTime.TimeOfDay} - {booking.EndTime.TimeOfDay} er blevet aflyst." +
                   "\nVi beklager ulejligheden." +
                   "\n\nMVH," +
                   "\nLenes Klinik.";
        }


        /// <summary>
        /// Returns an array of the dates of the workweek (mon-fri).
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
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