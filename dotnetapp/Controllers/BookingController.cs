using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetapp.Exceptions;
using dotnetapp.Models;

namespace Tablebooking.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public BookingController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            // Write a function to return a DinningTables in a List format 
            return null;
        }

        [HttpGet]
        public IActionResult Create(int tableId)
        {
            // Write a function to return the DinningTables table with Bookings table
                // If table is null return not found
                // If table Availability is false throw an exception
            return null;
        }

        [HttpPost]
        public IActionResult Create(int tableId, DateTime reservationDate, TimeSpan timeSlot)
        {
            // Write your code here to add new booking:
                // If the table is not found, return a 404 error.
                // If the table is already booked, throw an exception.
                // Check if the reservation date is after july 1st 2023. throw exception
                // Check if the table is already booked for the selected time slot. throw exception
                // Create a new booking and save it to the database.
                // Redirect to the confirmation page.
            return null;
        }

        public IActionResult Confirmation(int bookingId)
        {
            // Write your code here to Confirm your booking:
                //Get the booking from the database.
                // If the booking is not found, return a 404 error.
                // Return the view with the booking.
            return null;
        }
    }
}
