using System;
using System.Collections.Generic;
using System.Linq;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Data.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private DataContext _ctx;


        public BookingRepository(DataContext ctx)
        {
            _ctx = ctx;
        }

        public List<Booking> getBookingsByDate(DateTime dateTime)
        {
            return _ctx.Bookings.Where(book => book.StartTime.Date == dateTime.Date).ToList();
        }
    }
}