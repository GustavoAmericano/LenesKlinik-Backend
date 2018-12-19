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
        private readonly IBookingService _service;
        private readonly Mock<IBookingRepository> _mockBook;
        private readonly Mock<IWorkRepository> _mockWork;
        private Mock<IEmailService> _mockMail;

        private DateTime _date;

        public BookingServiceTest()
        {
            // Since it's currently hardcoded that a week is 5 days(Mon - Fri), we need to ensure we don't run tests
            // For either sunday or saturday. 
            _date = DateTime.Now;
            if ((int)_date.DayOfWeek  == 0 || (int)_date.DayOfWeek == 6) _date = _date.AddDays(2); 

            _mockBook = new Mock<IBookingRepository>();
            _mockWork = new Mock<IWorkRepository>();
            _mockMail = new Mock<IEmailService>();
            _service = new BookingService(_mockBook.Object, _mockWork.Object, _mockMail.Object);


            // Mock GetBookingByDate to return any booking where startTime is the given Date in the request.
            _mockBook.Setup(repo => repo.GetBookingsByDate(It.IsAny<DateTime>()))
                .Returns<DateTime>(dt => GetMockBookings()
                .Where(book => book.StartTime.Date == dt.Date).ToList());

            // Mock SaveBooking to modify the id of the given Booking and return that entity back.
            _mockBook.Setup(repo => repo.SaveBooking(It.IsAny<Booking>())).Returns<Booking>(book => {
                book.Id = 1;
                return book;
            });

            //Mock GetBookingsByCustomer id to return all bookings with the given customerId.
            _mockBook.Setup(repo => repo.GetBookingsByCustomerId(It.IsAny<int>()))
                .Returns<int>(id => GetMockBookings().Where(x => x.Customer.Id == id).ToList());

            //Mock GetWorkById in Work repo to return any work in the GetMockWork with the given id.
            _mockWork.Setup(repo => repo.GetWorkById(It.IsAny<int>())).Returns<int>(id => GetMockWork()[id-1]);
        }

#region SAVE
        [Fact]
        public void SaveBookingSuccessTest()
        {

            Booking booking = new Booking
            {
                StartTime = new DateTime(_date.Year, _date.Month, _date.Day, 09, 00, 00),
                EndTime = new DateTime(_date.Year, _date.Month, _date.Day, 10, 45, 00),
                Customer = new Customer{Id = 1, User = new User{Id = 1}},
                Work = new Work { Id = 1}
            };
            booking = _service.SaveBooking(booking);
            _mockBook.Verify(repo => repo.SaveBooking(It.IsAny<Booking>()), Times.Once());
            Assert.Equal(1, booking.Id);
        }

        [Fact]
        public void SaveBookingStartTimeNotDivisableBy15ExpectArgumentExceptionTest()
        {
            Booking booking = new Booking
            {
                StartTime = new DateTime(_date.Year, _date.Month, _date.Day, 09, 43, 00),
                EndTime = new DateTime(_date.Year, _date.Month, _date.Day, 10, 45, 00),
                Customer = new Customer { Id = 1, User = new User { Id = 1 } },
                Work = new Work { Id = 1 }
            };
            Exception e = Assert.Throws<ArgumentException>(() =>_service.SaveBooking(booking));
            _mockBook.Verify(repo => repo.SaveBooking(It.IsAny<Booking>()), Times.Never);
            Assert.Equal("Invalid start time!", e.Message);
        }

        [Fact]
        public void SaveBookingEndTimeNotDivisableBy15ExpectArgumentExceptionTest()
        {
            Booking booking = new Booking
            {
                StartTime = new DateTime(_date.Year, _date.Month, _date.Day, 09, 45, 00),
                EndTime = new DateTime(_date.Year, _date.Month, _date.Day, 10, 43, 00),
                Customer = new Customer { Id = 1, User = new User { Id = 1 } },
                Work = new Work { Id = 1 }
            };
            Exception e = Assert.Throws<ArgumentException>(() => _service.SaveBooking(booking));
            _mockBook.Verify(repo => repo.SaveBooking(It.IsAny<Booking>()), Times.Never);
            Assert.Equal("Invalid end time!", e.Message);
        }

        [Fact]
        public void SaveBookingEndTimeBeforeStartTime()
        {
            Booking booking = new Booking
            {
                StartTime = new DateTime(_date.Year, _date.Month, _date.Day, 10, 45, 00),
                EndTime = new DateTime(_date.Year, _date.Month, _date.Day, 09, 45, 00),
                Customer = new Customer { Id = 1, User = new User { Id = 1 } },
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
            var workId = 1;

            var allAvailableBookings = _service.GetAvailableBookings(_date, workId);

            int todayAsInt = (int) _date.DayOfWeek - 1;
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
            _date = _date.AddYears(-1);
            var workId = 1;
            
            Exception e = Assert.Throws<ArgumentException>(() => _service.GetAvailableBookings(_date, workId));

            Assert.Equal("Date was before today!",e.Message);
        }

        #endregion

        #region GET

        [Fact]
        public void GetAllBookingTest()
        {
            List<BookingInfo> bookings = _service.GetBookingsForWeek(_date);
            _mockBook.Verify(repo => repo.GetBookingsByDate(It.IsAny<DateTime>()), Times.AtLeastOnce);
            Assert.Equal(5, bookings.Count);
            Assert.Equal(2, bookings[(int) _date.DayOfWeek - 1].Bookings.Count); // Ensure that today's date includes the
            // two mock bookings.
        }

        [Fact]
        public void GetBookingsByCustomerIdSuccessTest()
        {
            List<Booking> bookings = _service.GetBookingsByCustomerId(1);

            _mockBook.Verify(repo => repo.GetBookingsByCustomerId(It.IsAny<int>()), Times.Once);
            Assert.Equal(1, bookings.Count);
        }

        [Fact]
        public void GetBookingsByCustomerIdNoBookingsExpectNullTest()
        {
            List<Booking> bookings = _service.GetBookingsByCustomerId(3);

            _mockBook.Verify(repo => repo.GetBookingsByCustomerId(It.IsAny<int>()), Times.Once());
            Assert.Null(bookings);
        }
#endregion

        private List<Booking> GetMockBookings()
        {

            return new List<Booking>()
            {

                new Booking
                {
                    Id = 1,
                    Customer = new Customer{Id = 1, User = new User{Id = 1}},
                    Work = new Work { Id = 1},
                    StartTime = new DateTime(_date.Year, _date.Month, _date.Day, 9,45,0),
                    EndTime = new DateTime(_date.Year, _date.Month, _date.Day, 9,45,0).AddMinutes(45),
                },
                new Booking
                {
                    Id = 2,
                    Customer = new Customer{Id = 2, User = new User{Id = 2}},
                    Work = new Work { Id = 1},
                    StartTime = new DateTime(_date.Year, _date.Month, _date.Day, 11,00,0),
                    EndTime = new DateTime(_date.Year, _date.Month, _date.Day, 11,00,0).AddMinutes(30),
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