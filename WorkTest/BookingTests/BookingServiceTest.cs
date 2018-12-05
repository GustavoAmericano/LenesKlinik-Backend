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
        }

        [Fact]
        public void GetAvailableBookingsSuccessTest()
        {
            var date = DateTime.Now;
            var duration = 45;

            var array = _service.GetAvailableBookings(date, duration);

            _mock.Verify(repo => repo.getBookingsByDate(It.IsAny<DateTime>()), Times.Exactly(7));

            Assert.Equal(21, array[(int) date.DayOfWeek - 1].Count);

            Assert.True(array[(int)date.DayOfWeek - 1][0].StartTime.TimeOfDay.Equals(new TimeSpan(09, 00, 00)));
            Assert.True(array[(int)date.DayOfWeek - 1][1].StartTime.TimeOfDay.Equals(new TimeSpan(11, 30, 00)));
            Assert.True(array[(int)date.DayOfWeek - 1][array[(int)date.DayOfWeek - 1].Count-1]
                .StartTime.TimeOfDay.Equals(new TimeSpan(16, 15, 00)));

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

        private List<Booking> GetMockBookings()
        {
            DateTime date = DateTime.Now;

            return new List<Booking>()
            {

                new Booking
                {
                    Id = 1,
                    UserId = 1,
                    WorkId = 1,
                    StartTime = new DateTime(date.Year, date.Month, date.Day, 9,45,0),
                    endTime = new DateTime(date.Year, date.Month, date.Day, 9,45,0).AddMinutes(45),
                },
                new Booking
                {
                    Id = 2,
                    UserId = 1,
                    WorkId = 1,
                    StartTime = new DateTime(date.Year, date.Month, date.Day, 11,00,0),
                    endTime = new DateTime(date.Year, date.Month, date.Day, 11,00,0).AddMinutes(30),
                },

            };
        }
    }
}