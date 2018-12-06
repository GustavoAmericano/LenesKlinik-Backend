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
        private readonly Mock<IBookingRepository> _mockBook;
        private readonly Mock<IWorkRepository> _mockWork;

        public BookingServiceTest()
        {
            _mockBook = new Mock<IBookingRepository>();
            _mockWork = new Mock<IWorkRepository>();
            _service = new BookingService(_mockBook.Object, _mockWork.Object);
            _bookings = new List<Booking>();
            _mockBook.Setup(repo => repo.GetBookingsByDate(It.IsAny<DateTime>()))
                .Returns<DateTime>(dt => GetMockBookings()
                .Where(book => book.StartTime.Date == dt.Date).ToList());
            _mockBook.Setup(repo => repo.SaveBooking(It.IsAny<Booking>())).Returns<Booking>(book => {
                book.Id = 1;
                return book;
            });
            _mockWork.Setup(repo => repo.GetWorkById(It.IsAny<int>())).Returns<int>(id => GetMockWork()[id-1]);
        }

#region SAVE
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
            _mockBook.Verify(repo => repo.SaveBooking(It.IsAny<Booking>()), Times.Once());
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
            _mockBook.Verify(repo => repo.SaveBooking(It.IsAny<Booking>()), Times.Never);
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
            _mockBook.Verify(repo => repo.SaveBooking(It.IsAny<Booking>()), Times.Never);
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
            _mockBook.Verify(repo => repo.SaveBooking(It.IsAny<Booking>()), Times.Never);
            Assert.Equal("Invalid time - End before start!", e.Message);
        }
#endregion
        
#region GetAvailableBookings
        [Fact]
        public void GetAvailableBookingsSuccessTest()
        {
            var date = DateTime.Now;
            var workId = 1;

            var allAvailableBookings = _service.GetAvailableBookings(date, workId);

            int todayAsInt = (int) date.DayOfWeek - 1;
            var availableBookings = allAvailableBookings[todayAsInt].AvailableSessions;
            
            _mockBook.Verify(repo => repo.GetBookingsByDate(It.IsAny<DateTime>()), Times.AtLeastOnce);
            
            Assert.Equal(21, allAvailableBookings[todayAsInt].AvailableSessions.Count);

            Assert.True(availableBookings[0].StartTime.TimeOfDay.Equals(new TimeSpan(09, 00, 00)));
            Assert.True(availableBookings[1].StartTime.TimeOfDay.Equals(new TimeSpan(11, 30, 00)));
            Assert.True(availableBookings[availableBookings.Count -1].StartTime.TimeOfDay.Equals(new TimeSpan(16, 15, 00)));

        }

        [Fact]
        public void GetAvailableBookingsWrongDateExpectArgumentExceptionTest()
        {
            var date = DateTime.Now.AddDays(-1);
            var workId = 1;
            
            Exception e = Assert.Throws<ArgumentException>(() => _service.GetAvailableBookings(date, workId));

            Assert.Equal("Date was before today!",e.Message);

        }

        [Fact]
        public void GetAvailableBookingsWrongDurationExpectArgumentExceptionTest()
        {
            var date = DateTime.Now;
            var workId = 3;

            Exception e = Assert.Throws<ArgumentException>(() => _service.GetAvailableBookings(date, workId));

            Assert.Equal("Duration must be divisible by 15", e.Message);
        }

        #endregion

        #region GET

        [Fact]
        public void GetAllBookingTest()
        {
            List<BookingInfo> bookings = _service.GetBookingsForWeek(DateTime.Now);
            _mockBook.Verify(repo => repo.GetBookingsByDate(It.IsAny<DateTime>()), Times.AtLeastOnce);
            Assert.Equal(5, bookings.Count);
            Assert.Equal(2, bookings[(int) DateTime.Today.DayOfWeek - 1].Bookings.Count); // Ensure that todays date includes the
            // two mock bookings.
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

        private List<Work> GetMockWork()
        {
            return new List<Work>()
            {
                new Work
                {
                    Id = 1,
                    Title = "Massage - short",
                    Price = 2500,
                    Description = "A short massage",
                    Duration = 45,
                    Bookings = null,
                    ImageUrl = "url.png"
                },
                new Work
                {
                    Id = 2,
                    Title = "Massage - long",
                    Price = 2500,
                    Description = "A long massage",
                    Duration = 60,
                    Bookings = null,
                    ImageUrl = "url.png"
                },
                new Work
                {
                    Id = 3,
                    Title = "Massage - Errorous",
                    Price = 2500,
                    Description = "A wrong massage",
                    Duration = 37,
                    Bookings = null,
                    ImageUrl = "url.png"
                },
            };
        }
    }
}