using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using dotnetapp.Controllers;
using dotnetapp.Models;
using dotnetapp.Exceptions;
using System.Reflection;
// using Moq;


namespace dotnetapp.Tests
{
    [TestFixture]
    public class TableControllerTests
    {
        private ApplicationDbContext _dbContext;
        private TableController _tableController;
        private BookingController _bookingController;
        private Type programType = typeof(BookingController);


        [SetUp]
        public void Setup()
        {
            // Initialize a new in-memory ApplicationDbContext for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _tableController = new TableController(_dbContext);
            _bookingController = new BookingController(_dbContext);

        }

        [TearDown]
        public void TearDown()
        {
            // Dispose the ApplicationDbContext and reset the database
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        private MethodInfo GetConfirmationMethodInfo()
        {
            // Get the type of the BookingController class
            Type controllerType = typeof(BookingController);

            // Get the method info for the Confirmation method with the required parameters
            MethodInfo confirmationMethod = controllerType.GetMethod("Confirmation", new[] { typeof(int) });

            // Return the method info
            return confirmationMethod;
        }

        private MethodInfo GetAvailableSlotsMethodInfo()
        {
            Type controllerType = typeof(TableController);

            MethodInfo availableSlotsMethod = controllerType.GetMethod("AvailableSlots", Type.EmptyTypes);

            // Return the method info
            return availableSlotsMethod;
        }

        private MethodInfo GetBookedSlotsMethodInfo()
        {
            // Get the type of the TableController class
            Type controllerType = typeof(TableController);

            // Get the method info for the BookedSlots action (no parameters required)
            MethodInfo bookedSlotsMethod = controllerType.GetMethod("BookedSlots", Type.EmptyTypes);

            // Return the method info
            return bookedSlotsMethod;
        }

        private MethodInfo GetCreateMethodInfo(bool isHttpPost)
        {
            var method = programType.GetMethods()
                .FirstOrDefault(m =>
                    m.Name == "Create" &&
                    m.GetParameters().Length == (isHttpPost ? 3 : 1) &&
                    (isHttpPost
                        ? m.GetParameters()[0].ParameterType == typeof(int) &&
                          m.GetParameters()[1].ParameterType == typeof(DateTime) &&
                          m.GetParameters()[2].ParameterType == typeof(TimeSpan)
                        : m.GetParameters()[0].ParameterType == typeof(int)
                    ));

            Assert.IsNotNull(method, "Create method not found.");
            return method;
        }

        [Test]
public void AvailableSlots_ReturnsViewResult()
{
    // Arrange
    var table1 = new DinningTable { DinningTableID = 1, SeatingCapacity = 4, Availability = true };
    var table2 = new DinningTable { DinningTableID = 2, SeatingCapacity = 2, Availability = true };
    _dbContext.DinningTables.AddRange(table1, table2);
    _dbContext.SaveChanges();

    // Get the MethodInfo for the AvailableSlots action
    var availableSlotsMethod = GetAvailableSlotsMethodInfo();

    // Act
    var result = availableSlotsMethod.Invoke(_tableController, null) as ViewResult;

    // Assert
    Assert.IsNotNull(result);
}


        [Test]
public void AvailableSlots_ReturnsViewResult_WithListOfAvailableTables()
{
    // Arrange
    var table1 = new DinningTable { DinningTableID = 1, SeatingCapacity = 4, Availability = true };
    var table2 = new DinningTable { DinningTableID = 2, SeatingCapacity = 2, Availability = true };
    _dbContext.DinningTables.AddRange(table1, table2);
    _dbContext.SaveChanges();

    // Get the MethodInfo for the AvailableSlots action
    var availableSlotsMethod = GetAvailableSlotsMethodInfo();

    // Act
    var result = availableSlotsMethod.Invoke(_tableController, null) as ViewResult;

    // Assert            
    var model = result.Model as List<DinningTable>;
    Assert.IsNotNull(model);
    Assert.AreEqual(2, model.Count);
    CollectionAssert.Contains(model, table1);
    CollectionAssert.Contains(model, table2);
}

[Test]
public void BookedSlots_ReturnsViewResult()
{
    // Arrange
    var table1 = new DinningTable { DinningTableID = 1, SeatingCapacity = 4, Availability = false };
    var table2 = new DinningTable { DinningTableID = 2, SeatingCapacity = 2, Availability = false };
    _dbContext.DinningTables.AddRange(table1, table2);
    _dbContext.SaveChanges();

    // Get the MethodInfo for the BookedSlots action
    var bookedSlotsMethod = GetBookedSlotsMethodInfo();

    // Act
    var result = bookedSlotsMethod.Invoke(_tableController, null) as ViewResult;

    // Assert
    Assert.IsNotNull(result);
}


        [Test]
public void BookedSlots_ReturnsViewResult_WithListOfBookedTables()
{
    // Arrange
    var table1 = new DinningTable { DinningTableID = 1, SeatingCapacity = 4, Availability = false };
    var table2 = new DinningTable { DinningTableID = 2, SeatingCapacity = 2, Availability = false };
    _dbContext.DinningTables.AddRange(table1, table2);
    _dbContext.SaveChanges();

    // Get the MethodInfo for the BookedSlots action
    var bookedSlotsMethod = GetBookedSlotsMethodInfo();

    // Act
    var result = bookedSlotsMethod.Invoke(_tableController, null) as ViewResult;

    // Assert
    var model = result.Model as List<DinningTable>;
    Assert.IsNotNull(model);
    Assert.AreEqual(2, model.Count);
    CollectionAssert.Contains(model, table1);
    CollectionAssert.Contains(model, table2);
}

    


       [Test]
public void Create_Get_ReturnsViewResult()
{
    // Arrange
    int tableId = 1;
    var table = new DinningTable { DinningTableID = tableId, SeatingCapacity = 4, Availability = true };
    _dbContext.DinningTables.Add(table);
    _dbContext.SaveChanges();

    // Get the MethodInfo for the Create method
    var createMethod = GetCreateMethodInfo(isHttpPost: false);

    // Act
    var result = createMethod.Invoke(_bookingController, new object[] { tableId });

    // Assert
    Assert.IsNotNull(result);
    Assert.IsInstanceOf<ViewResult>(result);

    var viewResult = result as ViewResult;
    Assert.IsNotNull(viewResult.Model);
    Assert.IsInstanceOf<DinningTable>(viewResult.Model);
}





       [Test]
public void Create_Get_InvalidTableId_ReturnsNotFound()
{
    // Arrange
    int tableId = 999;

    var createMethod = GetCreateMethodInfo(isHttpPost: false);

    // Act
    var result = createMethod.Invoke(_bookingController, new object[] { tableId });

    // Assert
    Assert.IsNotNull(result);
    Assert.IsInstanceOf<NotFoundResult>(result);
}

        [Test]
public void Create_Post_ValidBooking_Success()
{
    // Arrange
    int tableId = 1;
    var table = new DinningTable { DinningTableID = tableId, SeatingCapacity = 4, Availability = true };
    DateTime reservationDate = new DateTime(2023, 7, 5);
    TimeSpan timeSlot = TimeSpan.FromHours(10);
    _dbContext.DinningTables.Add(table);
    _dbContext.SaveChanges();

    // Get the MethodInfo for the Create method
    var createMethod = GetCreateMethodInfo(isHttpPost: true);

    // Act
    var result = createMethod.Invoke(_bookingController, new object[] { tableId, reservationDate, timeSlot }) as RedirectToActionResult;

    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual("Confirmation", result.ActionName);

    var booking = _dbContext.Bookings.Include(b => b.DinningTable).FirstOrDefault();
    Assert.IsNotNull(booking);
    Assert.AreEqual(tableId, booking.DinningTable.DinningTableID);
    Assert.AreEqual(reservationDate.Date, booking.ReservationDate.Date);
    Assert.AreEqual(timeSlot, booking.TimeSlot);
    Assert.IsFalse(booking.DinningTable.Availability);
}


       [Test]
public void Create_Post_InvalidTableId_ReturnsNotFound()
{
    // Arrange
    int tableId = 999; // Invalid table ID (non-existing)
    DateTime reservationDate = DateTime.Now.AddDays(1);
    TimeSpan timeSlot = TimeSpan.FromHours(10);

    // Get the MethodInfo for the Create method
    var createMethod = GetCreateMethodInfo(isHttpPost: true);

    // Act
    var result = createMethod.Invoke(_bookingController, new object[] { tableId, reservationDate, timeSlot });

    // Assert
    Assert.IsNotNull(result);
    Assert.IsInstanceOf<NotFoundResult>(result);
}


        

        [Test]
public void Create_Post_TableAlreadyBooked_ThrowsException()
{
    // Arrange
    int tableId = 1;
    var table = new DinningTable { DinningTableID = tableId, SeatingCapacity = 4, Availability = false };
    _dbContext.DinningTables.Add(table);
    _dbContext.SaveChanges();

    // Get the MethodInfo for the Create method
    var createMethod = GetCreateMethodInfo(isHttpPost: false);

    // Act & Assert
    var ex = Assert.Throws<TargetInvocationException>(() => createMethod.Invoke(_bookingController, new object[] { tableId }));
    Assert.IsNotNull(ex.InnerException);
    Assert.IsInstanceOf<TableBookingException>(ex.InnerException);
    Assert.AreEqual("Table already booked", ex.InnerException.Message);
}



        [Test]
        public void Create_Post_TableAlreadyBooked_ThrowsException_with_Message()
        {
             // Arrange
    int tableId = 1;
    var table = new DinningTable { DinningTableID = tableId, SeatingCapacity = 4, Availability = false };
    _dbContext.DinningTables.Add(table);
    _dbContext.SaveChanges();

    // Get the MethodInfo for the Create method
    var createMethod = GetCreateMethodInfo(isHttpPost: false);

    // Act & Assert
    var ex = Assert.Throws<TargetInvocationException>(() => createMethod.Invoke(_bookingController, new object[] { tableId }));
    Assert.AreEqual("Table already booked", ex.InnerException.Message);
        }

        [Test]
        public void Create_Post_InvalidReservationDate_ThrowsException()
        {
            // Arrange
    int tableId = 1;
    var table = new DinningTable { DinningTableID = tableId, SeatingCapacity = 4, Availability = true };
    DateTime reservationDate = new DateTime(2023, 1, 1);
    TimeSpan timeSlot = TimeSpan.FromHours(10);
    _dbContext.DinningTables.Add(table);
    _dbContext.SaveChanges();

    // Get the MethodInfo for the Create method
    var createMethod = GetCreateMethodInfo(isHttpPost: true);

    // Act & Assert
    var ex = Assert.Throws<TargetInvocationException>(() => createMethod.Invoke(_bookingController, new object[] { tableId, reservationDate, timeSlot }));
    var tableBookingEx = ex.InnerException as TableBookingException;

    Assert.IsNotNull(tableBookingEx);
        }

        [Test]
public void Create_Post_InvalidReservationDate_ThrowsException_with_message()
{
    // Arrange
    int tableId = 1;
    var table = new DinningTable { DinningTableID = tableId, SeatingCapacity = 4, Availability = true };
    DateTime reservationDate = new DateTime(2023, 1, 1);
    TimeSpan timeSlot = TimeSpan.FromHours(10);
    _dbContext.DinningTables.Add(table);
    _dbContext.SaveChanges();

    // Get the MethodInfo for the Create method
    var createMethod = GetCreateMethodInfo(isHttpPost: true);

    // Act & Assert
    var ex = Assert.Throws<TargetInvocationException>(() => createMethod.Invoke(_bookingController, new object[] { tableId, reservationDate, timeSlot }));
    var tableBookingEx = ex.InnerException as TableBookingException;
    Assert.AreEqual("Invalid reservation date", tableBookingEx.Message);
}



        [Test]
public void Confirmation_InvalidBookingId_ReturnsNotFound()
{
    // Arrange
    int bookingId = 999; // Invalid booking ID (non-existing)

    // Get the MethodInfo for the Confirmation method
    var confirmationMethod = GetConfirmationMethodInfo();

    // Act
    var result = confirmationMethod.Invoke(_bookingController, new object[] { bookingId });

    // Assert
    Assert.IsNotNull(result);
    Assert.IsInstanceOf<NotFoundResult>(result);
}


         [Test]
        public void Booking_Properties_BookingID_GetSetCorrectly()
        {
            var booking = new Booking();

            Type bookingType = typeof(Booking);
            PropertyInfo idProperty = bookingType.GetProperty("BookingID");

            if (idProperty != null && idProperty.CanWrite)
            {
                idProperty.SetValue(booking, 1);
            }

            object expectedValue = 1;
            object actualValue = idProperty.GetValue(booking);

            Assert.AreEqual(expectedValue, actualValue);

            // Additional assertions for PropertyInfo
            Assert.NotNull(idProperty);
            Assert.AreEqual(typeof(int), idProperty.PropertyType);
            Assert.IsTrue(idProperty.CanRead); // Ensure the property can be read
            Assert.IsTrue(idProperty.CanWrite); // Ensure the property can be written
    
        }
        [Test]
        public void Booking_Properties_DinningTableID_GetSetCorrectly()
        {
            // Arrange
    var booking = new Booking();

    Type bookingType = typeof(Booking);
    PropertyInfo dinningTableIdProperty = bookingType.GetProperty("DinningTableID");

    // Act
    dinningTableIdProperty.SetValue(booking, 2);

    // Assert
    Assert.AreEqual(2, dinningTableIdProperty.GetValue(booking));
        }

        [Test]
        public void Booking_Properties_ReservationDate_GetSetCorrectly()
        {
    // Arrange
    var booking = new Booking();

    Type bookingType = typeof(Booking);
    PropertyInfo reservationDateProperty = bookingType.GetProperty("ReservationDate");

    // Act
    DateTime reservationDateValue = new DateTime(2023, 7, 1);
    reservationDateProperty.SetValue(booking, reservationDateValue);

    // Assert
    DateTime actualReservationDateValue = (DateTime)reservationDateProperty.GetValue(booking);
    Assert.AreEqual(reservationDateValue, actualReservationDateValue);
}

        [Test]
        public void Booking_Properties_TimeSlot_GetSetCorrectly()
        {
            // Arrange
            var booking = new Booking();

            Type bookingType = typeof(Booking);
            PropertyInfo timeSlotProperty = bookingType.GetProperty("TimeSlot");

            // Act
            TimeSpan timeSlotValue = new TimeSpan(14, 0, 0);
            timeSlotProperty.SetValue(booking, timeSlotValue);

            // Assert
            TimeSpan actualTimeSlotValue = (TimeSpan)timeSlotProperty.GetValue(booking);
            Assert.AreEqual(timeSlotValue, actualTimeSlotValue);
        }

        [Test]
public void TestBookingProperties1() { 
    Type employeeType = typeof(Booking); 
    PropertyInfo idProperty = employeeType.GetProperty("BookingID"); 
    // PropertyInfo nameProperty = employeeType.GetProperty("Name"); 
    Assert.NotNull(idProperty); 
    Assert.AreEqual(typeof(int), idProperty.PropertyType); 
    // Assert.NotNull(nameProperty); 
    // Assert.AreEqual(typeof(string), nameProperty.PropertyType); 
    }
        [Test]
        public void Booking_Properties_DinningTableID_HaveCorrectDataTypes()
        {
            // Arrange
            var booking = new Booking();

            Type bookingType = typeof(Booking);
            PropertyInfo dinningTableIdProperty = bookingType.GetProperty("DinningTableID");

            // Assert
            Assert.NotNull(dinningTableIdProperty);
            Assert.AreEqual(typeof(int), dinningTableIdProperty.PropertyType);
        }

        [Test]
        public void Booking_Properties_ReservationDate_HaveCorrectDataTypes()
        {
            var booking = new Booking();

            Type bookingType = typeof(Booking);
            PropertyInfo reservationDateProperty = bookingType.GetProperty("ReservationDate");
        
            // Assert
            Assert.NotNull(reservationDateProperty);
            Assert.AreEqual(typeof(DateTime), reservationDateProperty.PropertyType);            
        }

        [Test]
        public void Booking_Properties_TimeSlot_HaveCorrectDataTypes()
        {
            // Arrange
            var booking = new Booking();

            Type bookingType = typeof(Booking);
            PropertyInfo timeSlotProperty = bookingType.GetProperty("TimeSlot");

            // Assert
            Assert.NotNull(timeSlotProperty);
            Assert.AreEqual(typeof(TimeSpan), timeSlotProperty.PropertyType);
        }

        
        [Test]
        public void DinningTableClassExists()
        {
            var dinningTable = new DinningTable();
        
            Assert.IsNotNull(dinningTable);
        }
        
        [Test]
        public void BookingClassExists()
        {
            var booking = new Booking();
        
            Assert.IsNotNull(booking);
        }
        
        [Test]
        public void ApplicationDbContextContainsDbSetSlotProperty()
        {
            // using (var context = new ApplicationDbContext(_dbContextOptions))
            //         {
            // var context = new ApplicationDbContext();
        
            var propertyInfo = _dbContext.GetType().GetProperty("Bookings");
        
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(typeof(DbSet<Booking>), propertyInfo.PropertyType);
                    // }
        }
        
        [Test]
        public void ApplicationDbContextContainsDbSetBookingPropertyInfo()
        {
            // using (var context = new ApplicationDbContext(_dbContextOptions))
            //         {
            // var context = new ApplicationDbContext();
        
            var propertyInfo = _dbContext.GetType().GetProperty("DinningTables");
        
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(typeof(DbSet<DinningTable>), propertyInfo.PropertyType);
        }

        [Test]
        public void ApplicationDbContextContainsDbSetBookingProperty()
        {
            // using (var context = new ApplicationDbContext(_dbContextOptions))
            //         {
            // var context = new ApplicationDbContext();
        
            var propertyInfo = _dbContext.GetType().GetProperty("DinningTables");
        
            Assert.AreEqual(typeof(DbSet<DinningTable>), propertyInfo.PropertyType);
        }

        [Test]
        public void DinningTable_Properties_GetSetCorrectly()
        {
            var dinningTable = new DinningTable();

            Type dinningTableType = typeof(DinningTable);
            PropertyInfo idProperty = dinningTableType.GetProperty("DinningTableID");
            PropertyInfo seatingCapacityProperty = dinningTableType.GetProperty("SeatingCapacity");

            // Act
            idProperty.SetValue(dinningTable, 1);
            seatingCapacityProperty.SetValue(dinningTable, 4);

            // Assert
            Assert.AreEqual(1, idProperty.GetValue(dinningTable));
            Assert.AreEqual(4, seatingCapacityProperty.GetValue(dinningTable));
        }

        [Test]
        public void DinningTable_Properties_Availability_GetSetCorrectly()
        {
            // Arrange
            var dinningTable = new DinningTable();

            Type dinningTableType = typeof(DinningTable);
            PropertyInfo availabilityProperty = dinningTableType.GetProperty("Availability");

            // Act
            availabilityProperty.SetValue(dinningTable, true);

            // Assert
            Assert.IsTrue((bool)availabilityProperty.GetValue(dinningTable));
        }

        [Test]
        public void DinningTable_Properties_HaveCorrectDataTypes()
        {
            // Arrange
            var dinningTable = new DinningTable();

            Type dinningTableType = typeof(DinningTable);
            PropertyInfo idProperty = dinningTableType.GetProperty("DinningTableID");
            PropertyInfo seatingCapacityProperty = dinningTableType.GetProperty("SeatingCapacity");

            // Assert
            Assert.NotNull(idProperty);
            Assert.AreEqual(typeof(int), idProperty.PropertyType);

            Assert.NotNull(seatingCapacityProperty);
            Assert.AreEqual(typeof(int), seatingCapacityProperty.PropertyType);
        }
        [Test]
        public void DinningTable_Properties_Availability_HaveCorrectDataTypes()
        {
            var dinningTable = new DinningTable();

            Type dinningTableType = typeof(DinningTable);
            PropertyInfo availabilityProperty = dinningTableType.GetProperty("Availability");

            // Assert
            Assert.NotNull(availabilityProperty);
            Assert.AreEqual(typeof(bool), availabilityProperty.PropertyType);
        }

        



    }
}
