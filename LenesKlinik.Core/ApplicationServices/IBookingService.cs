using System;
using System.Collections.Generic;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices
{
    public interface IBookingService
    {
        List<AvailableBooking>[] GetAvailableBookings(DateTime date, int duration);
    }
}