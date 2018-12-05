using System;
using System.Collections.Generic;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.DomainServices
{
    public interface IBookingRepository
    {
        List<Booking> getBookingsByDate(DateTime dateTime);
        Booking SaveBooking(Booking booking);
    }
}