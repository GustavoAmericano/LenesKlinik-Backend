using System;
using System.Collections.Generic;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices
{
    public interface IBookingService
    {
        List<AvailableSessionsForDate> GetAvailableBookings(DateTime date, int duration);
    }
}