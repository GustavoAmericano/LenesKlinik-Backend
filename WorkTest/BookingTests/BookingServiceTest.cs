using System;
using System.Collections.Generic;
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
        private readonly List<Booking> _work;
        private readonly IBookingService _service;
        private readonly Mock<IBookingRepository> _mock;

        public BookingServiceTest()
        {
            _mock = new Mock<IBookingRepository>();
            _service = new BookingService(_mock.Object);
            _work = new List<Booking>();
        }
    }
}