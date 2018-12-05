using System;
using System.Collections.Generic;
using System.Linq;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
using Microsoft.EntityFrameworkCore;

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

        public Booking SaveBooking(Booking booking)
        {
            try
            {
                _ctx.Attach(booking).State = EntityState.Added;
                _ctx.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create entity!");
            }

            return booking;
        }

    }
}