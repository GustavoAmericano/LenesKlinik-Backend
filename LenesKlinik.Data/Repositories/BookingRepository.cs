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
        private readonly DataContext _ctx;


        public BookingRepository(DataContext ctx)
        {
            _ctx = ctx;
        }

        public List<Booking> GetBookingsByDate(DateTime dateTime)
        {
                return _ctx.Bookings.Where(book => book.StartTime.Date == dateTime.Date)
                    .Include(book => book.Customer)
                    .Include(book => book.Work)
                    .OrderBy(book => book.StartTime)
                    .ToList();
        }

        public List<Booking> GetBookingsByCustomerId(int customerId)
        {
                return _ctx.Bookings.Where(book => book.Customer.Id == customerId)
                    .Include(book => book.Customer)
                    .Include(book => book.Work)
                    .OrderBy(book => book.StartTime)
                    .ToList();
        }

        public Booking DeleteBooking(int bookingId)
        {
            Booking booking = _ctx.Bookings
                .Include(book => book.Customer)
                .ThenInclude(cust => cust.User)
                .FirstOrDefault(book => book.Id == bookingId);
            if (booking == null) return null;
            _ctx.Bookings.Remove(booking);
            _ctx.SaveChanges();
            return booking;
        }

        public Booking SaveBooking(Booking booking)
        {
                _ctx.Attach(booking).State = EntityState.Added;
                _ctx.SaveChanges();
                return booking;
        }

    }
}