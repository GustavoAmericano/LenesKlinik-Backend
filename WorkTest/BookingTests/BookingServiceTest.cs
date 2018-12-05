using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using LenesKlinik.Core.ApplicationServices;
using LenesKlinik.Core.ApplicationServices.Implementation;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
using Moq;
using Xunit;

namespace CoreTest.BookingTests
{
    public class BookingServiceTest
    {
        private readonly List<Booking> _bookings;
        private readonly IBookingService _service;
        private readonly Mock<IBookingRepository> _mock;

        public BookingServiceTest()
        {
            _mock = new Mock<IBookingRepository>();
            _service = new BookingService(_mock.Object);
            _bookings = new List<Booking>();
            _mock.Setup(repo => repo.getBookingsByDate(It.IsAny<DateTime>()))
                .Returns<DateTime>(dt => GetMockBookings()
                .Where(book => book.StartTime.Date == dt.Date).ToList());
            _mock.Setup(repo => repo.SaveBooking(It.IsAny<Booking>())).Returns<Booking>(book => {
                book.Id = 1;
                return book;
            });
        }

        [Fact]
        public void SaveBookingSuccessTest()
        {
            var today = DateTime.Today;
            Booking booking = new Booking
            {
                StartTime = new DateTime(today.Year, today.Month, today.Day, 09, 00, 00),
                EndTime = new DateTime(today.Year, today.Month, today.Day, 10, 45, 00),
                User = new User{Id = 1},
                Work = new Work { Id = 1}
            };
            booking = _service.SaveBooking(booking);
            _mock.Verify(repo => repo.SaveBooking(It.IsAny<Booking>()), Times.Once());
            Assert.Equal(1, booking.Id);
        }

        [Fact]
        public void SaveBookingStartTimeNotDivisableBy15ExpectArgumentExceptionTest()
        {
            var today = DateTime.Today;
            Booking booking = new Booking
            {
                StartTime = new DateTime(today.Year, today.Month, today.Day, 09, 43, 00),
                EndTime = new DateTime(today.Year, today.Month, today.Day, 10, 45, 00),
                User = new User { Id = 1 },
                Work = new Work { Id = 1 }
            };
            Exception e = Assert.Throws<ArgumentException>(() =>_service.SaveBooking(booking));
            _mock.Verify(repo => repo.SaveBooking(It.IsAny<Booking>()), Times.Never);
            Assert.Equal("Invalid start time!", e.Message);
        }

        [Fact]
        public void SaveBookingEndTimeNotDivisableBy15ExpectArgumentExceptionTest()
        {
            var today = DateTime.Today;
            Booking booking = new Booking
            {
                StartTime = new DateTime(today.Year, today.Month, today.Day, 09, 45, 00),
                EndTime = new DateTime(today.Year, today.Month, today.Day, 10, 43, 00),
                User = new User { Id = 1 },
                Work = new Work { Id = 1 }
            };
            Exception e = Assert.Throws<ArgumentException>(() => _service.SaveBooking(booking));
            _mock.Verify(repo => repo.SaveBooking(It.IsAny<Booking>()), Times.Never);
            Assert.Equal("Invalid end time!", e.Message);
        }

        [Fact]
        public void SaveBookingEndTimeBeforeStartTime()
        {
            var today = DateTime.Today;
            Booking booking = new Booking
            {
                StartTime = new DateTime(today.Year, today.Month, today.Day, 10, 45, 00),
                EndTime = new DateTime(today.Year, today.Month, today.Day, 09, 45, 00),
                User = new User { Id = 1 },
                Work = new Work { Id = 1 }
            };
            Exception e = Assert.Throws<ArgumentException>(() => _service.SaveBooking(booking));
            _mock.Verify(repo => repo.SaveBooking(It.IsAny<Booking>()), Times.Never);
            Assert.Equal("Invalid time - End before start!", e.Message);
        }

#region GetAvailableBookings
        [Fact]
        public void GetAvailableBookingsSuccessTest()
        {
            var date = DateTime.Now;
            var duration = 45;

            var allAvailableBookings = _service.GetAvailableBookings(date, duration);

            int todayAsInt = (int) date.DayOfWeek - 1;
            var availableBookings = allAvailableBookings[todayAsInt].AvailableSessions;
            
            _mock.Verify(repo => repo.getBookingsByDate(It.IsAny<DateTime>()), Times.AtLeastOnce);
            
            Assert.Equal(21, allAvailableBookings[todayAsInt].AvailableSessions.Count);

            Assert.True(availableBookings[0].StartTime.TimeOfDay.Equals(new TimeSpan(09, 00, 00)));
            Assert.True(availableBookings[1].StartTime.TimeOfDay.Equals(new TimeSpan(11, 30, 00)));
            Assert.True(availableBookings[availableBookings.Count -1].StartTime.TimeOfDay.Equals(new TimeSpan(16, 15, 00)));

        }

        [Fact]
        public void GetAvailableBookingsWrongDateExpectArgumentExceptionTest()
        {
            var date = DateTime.Now.AddDays(-1);
            var duration = 45;
            
            Exception e = Assert.Throws<ArgumentException>(() => _service.GetAvailableBookings(date, duration));

            Assert.Equal("Date was before today!",e.Message);

        }

        [Fact]
        public void GetAvailableBookingsWrongDurationExpectArgumentExceptionTest()
        {
            var date = DateTime.Now;
            var duration = 36;

            Exception e = Assert.Throws<ArgumentException>(() => _service.GetAvailableBookings(date, duration));

            Assert.Equal("Duration must be divisible by 15", e.Message);
        }

#endregion

        private List<Booking> GetMockBookings()
        {
            DateTime date = DateTime.Now;

            return new List<Booking>()
            {

                new Booking
                {
                    Id = 1,
                    User = new User{Id = 1},
                    Work = new Work { Id = 1},
                    StartTime = new DateTime(date.Year, date.Month, date.Day, 9,45,0),
                    EndTime = new DateTime(date.Year, date.Month, date.Day, 9,45,0).AddMinutes(45),
                },
                new Booking
                {
                    Id = 2,
                    User = new User{Id = 1},
                    Work = new Work { Id = 1},
                    StartTime = new DateTime(date.Year, date.Month, date.Day, 11,00,0),
                    EndTime = new DateTime(date.Year, date.Month, date.Day, 11,00,0).AddMinutes(30),
                },

            };
        }
    }
}