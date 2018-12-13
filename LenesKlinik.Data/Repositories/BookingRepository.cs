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

        public void DeleteBooking(int bookingId)
        {
                _ctx.Bookings.Remove(_ctx.Bookings.First(book => book.Id == bookingId));
                _ctx.SaveChanges();
        }

        public Booking SaveBooking(Booking booking)
        {
                _ctx.Attach(booking).State = EntityState.Added;
                _ctx.SaveChanges();
                return booking;
        }

    }
}