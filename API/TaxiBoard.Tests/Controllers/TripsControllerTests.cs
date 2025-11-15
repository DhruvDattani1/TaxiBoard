using Microsoft.AspNetCore.Mvc;
using TaxiBoard.Controllers;
using TaxiBoard.Data;
using TaxiBoard.Models;
using TaxiBoard.Tests.TestUtilities;
using TaxiBoard.DTOs;
using Xunit;

namespace TaxiBoard.Tests.Controllers
{
    public class TripsControllerTests
    {
        private TaxiBoardContext GetContext()
        {
            return TestDbContextFactory.Create();
        }

        [Fact]
        public async Task GetTrips_ReturnsBadRequest_WhenInvalidPagination()
        {
  
            var context = GetContext();
            var controller = new TripsController(context);

            var result = await controller.GetTrips(page: 0, pageSize: 50);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetTrips_ReturnsPagedTrips()
        {
            var context = GetContext();

            context.YellowTripData.Add(new YellowTripData
            {
                Id = 1,
                PickupDatetime = DateTime.UtcNow,
                DropoffDatetime = DateTime.UtcNow.AddMinutes(10),
                PassengerCount = 2,
                TripDistance = 1.2m,
                Vendor = new Vendor { VendorName = "TestVendor" },
                PaymentType = new PaymentType { PaymentDescription = "Cash" },
                PickupZone = new TaxiZone { Zone = "Manhattan" },
                DropoffZone = new TaxiZone { Zone = "Queens" },
                TotalAmount = 10.50m
            });

            context.SaveChanges();

            var controller = new TripsController(context);

            var result = await controller.GetTrips(page: 1, pageSize: 10);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<PagedResultDto<TripDto>>(ok.Value);

            Assert.Equal(1, data.TotalCount);
            Assert.Single(data.Data);

            var trip = data.Data.First();
            Assert.Equal("Manhattan", trip.PickupZone);
            Assert.Equal("Queens", trip.DropoffZone);
            Assert.Equal("Cash", trip.PaymentType);
            Assert.Equal("TestVendor", trip.VendorName);
        }

        [Fact]
        public async Task GetTrips_AppliesPassengerFilter()
        {
            var context = GetContext();

            context.YellowTripData.Add(new YellowTripData
            {
                Id = 1,
                PassengerCount = 1,
                PickupDatetime = DateTime.UtcNow,
                Vendor = new Vendor { VendorName = "V1" },
                PaymentType = new PaymentType { PaymentDescription = "Cash" },
                PickupZone = new TaxiZone { Zone = "Zone1" },
                DropoffZone = new TaxiZone { Zone = "Zone1" }
            });

            context.YellowTripData.Add(new YellowTripData
            {
                Id = 2,
                PassengerCount = 3,
                PickupDatetime = DateTime.UtcNow,
                Vendor = new Vendor { VendorName = "V2" },
                PaymentType = new PaymentType { PaymentDescription = "Card" },
                PickupZone = new TaxiZone { Zone = "Zone2" },
                DropoffZone = new TaxiZone { Zone = "Zone2" }
            });

            context.SaveChanges();

            var controller = new TripsController(context);

            var result = await controller.GetTrips(passengers: 3);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<PagedResultDto<TripDto>>(ok.Value);

            Assert.Single(dto.Data);
            Assert.Equal(3, dto.Data.First().PassengerCount);
        }

        [Fact]
        public async Task GetTrips_AppliesDateFilter()
        {
            var context = GetContext();
            var now = DateTime.UtcNow;

            context.YellowTripData.Add(new YellowTripData
            {
                Id = 1,
                PickupDatetime = now.AddDays(-5),
                Vendor = new Vendor { VendorName = "V1" },
                PaymentType = new PaymentType { PaymentDescription = "Cash" },
                PickupZone = new TaxiZone { Zone = "ZoneA" },
                DropoffZone = new TaxiZone { Zone = "ZoneA" }
            });

            context.YellowTripData.Add(new YellowTripData
            {
                Id = 2,
                PickupDatetime = now.AddDays(-1),
                Vendor = new Vendor { VendorName = "V2" },
                PaymentType = new PaymentType { PaymentDescription = "Card" },
                PickupZone = new TaxiZone { Zone = "ZoneB" },
                DropoffZone = new TaxiZone { Zone = "ZoneB" }
            });

            context.SaveChanges();

            var controller = new TripsController(context);

            var result = await controller.GetTrips(
                startDate: now.AddDays(-2),
                endDate: now
            );

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<PagedResultDto<TripDto>>(ok.Value);

            Assert.Single(dto.Data);
            Assert.Equal(2, dto.Data.First().Id);
        }

        [Theory]
        [InlineData(1, 50, -2, 0, 3, 2)]  
        [InlineData(1, 50, -10, -3, 1, 1)] 
        public async Task GetTrips_AppliesAllFiltersTogether(
            int page,
            int pageSize,
            int startOffsetDays,
            int endOffsetDays,
            int passengers,
            int expectedTripId)
        {
            var context = GetContext();
            var now = DateTime.UtcNow;

            context.YellowTripData.Add(new YellowTripData
            {
                Id = 1,
                PassengerCount = 1,
                PickupDatetime = now.AddDays(-5),
                Vendor = new Vendor { VendorName = "V1" },
                PaymentType = new PaymentType { PaymentDescription = "Cash" },
                PickupZone = new TaxiZone { Zone = "Z1" },
                DropoffZone = new TaxiZone { Zone = "Z1" }
            });

            context.YellowTripData.Add(new YellowTripData
            {
                Id = 2,
                PassengerCount = 3,
                PickupDatetime = now.AddDays(-1),
                Vendor = new Vendor { VendorName = "V2" },
                PaymentType = new PaymentType { PaymentDescription = "Card" },
                PickupZone = new TaxiZone { Zone = "Z2" },
                DropoffZone = new TaxiZone { Zone = "Z2" }
            });

            context.SaveChanges();

            var controller = new TripsController(context);

            DateTime? start = now.AddDays(startOffsetDays);
            DateTime? end = now.AddDays(endOffsetDays);

            var result = await controller.GetTrips(
                page,
                pageSize,
                start,
                end,
                passengers);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<PagedResultDto<TripDto>>(ok.Value);

            Assert.Single(dto.Data);
            Assert.Equal(expectedTripId, dto.Data.First().Id);
        }

    }
}
