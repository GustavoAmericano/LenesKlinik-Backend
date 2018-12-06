﻿using System;
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

        public List<Booking> GetBookingsByDate(DateTime dateTime)
        {
            try
            {
                return _ctx.Bookings.Where(book => book.StartTime.Date == dateTime.Date).ToList();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to fetch bookings from DB!");
            }
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