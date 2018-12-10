using System;
using System.Collections.Generic;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.DomainServices
{
    public interface IBookingRepository
    {
        Booking SaveBooking(Booking booking);
        List<Booking> GetBookingsByDate(DateTime date);
    }
}