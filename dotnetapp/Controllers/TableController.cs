using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetapp.Models;

namespace dotnetapp.Controllers
{
    public class TableController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public TableController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult AvailableSlots()
        {
            // Get all the dining tables that are available.
            // Return the view with the available tables.
            return null;
        }

        public IActionResult BookedSlots()
        {
            // Get all the dining tables that are not available.
            // Return the view with the booked tables.
            return null;
        }
    }
}
