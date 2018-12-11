using System;
using System.Collections.Generic;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices
{
    public interface IBookingService
    {
        /// <summary>
        /// Calculates and returns all available sessions for the week
        /// based on the given date, and the length of the chosen work.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="workId"></param>
        /// <returns>A list of DateSessions, one for each day mon - fri of the week</returns>
        List<DateSessions> GetAvailableBookings(DateTime date, int workId);

        /// <summary>
        /// Validates and saves a booking object to the database.
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>The saved booking</returns>
        Booking SaveBooking(Booking booking);

        /// <summary>
        /// Gets all bookings for the week, based on the given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>A list of BookingInfo, one for each day mon-fri of the week</returns>
        List<BookingInfo> GetBookingsForWeek(DateTime date);

        /// <summary>
        /// Gets all bookings related to the customer with the given Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>A list of Bookings</returns>
        List<Booking> GetBookingsByCustomerId(int customerId);
    }
}