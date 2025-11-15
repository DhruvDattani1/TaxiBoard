using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiBoard.Controllers;
using TaxiBoard.Data;
using TaxiBoard.Models;
using TaxiBoard.DTOs;

namespace TaxiBoard.Tests.Controllers
{
    public class AnalyticsControllerTests
    {
        private TaxiBoardContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<TaxiBoardContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TaxiBoardContext(options);
        }

        private AnalyticsController CreateController(TaxiBoardContext ctx)
        {
            return new AnalyticsController(ctx);
        }

        [Fact]
        public async Task GetTripAnalytics_ReturnsZeroDto_WhenNoData()
        {
            using var ctx = CreateContext();
            var controller = CreateController(ctx);

            var result = await controller.GetTripAnalytics();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TripAnalyticsDto>(ok.Value);

            Assert.Equal(0, dto.TotalTrips);
            Assert.Equal(0, dto.AverageFare);
            Assert.Equal(0, dto.AverageDistance);
            Assert.Equal(0, dto.TotalRevenue);
            Assert.Equal("No Data", dto.MostPopularPickupZone);
            Assert.Equal("No Data", dto.MostUsedPaymentType);
        }


        [Fact]
        public async Task GetTripAnalytics_AppliesStartDateFilter()
        {
            using var ctx = CreateContext();

            var zone = new TaxiZone { Zone = "TestZone" };
            var payment = new PaymentType { PaymentDescription = "Cash" };

            ctx.TaxiZones.Add(zone);
            ctx.PaymentTypes.Add(payment);

            ctx.YellowTripData.AddRange(
                new YellowTripData
                {
                    PickupDatetime = new DateTime(2025, 01, 01),
                    TripDistance = 1.0m,
                    TotalAmount = 10,
                    PickupZone = zone,
                    PaymentType = payment
                },
                new YellowTripData
                {
                    PickupDatetime = new DateTime(2025, 03, 01),
                    TripDistance = 2.0m,
                    TotalAmount = 20,
                    PickupZone = zone,
                    PaymentType = payment
                }
            );

            await ctx.SaveChangesAsync();

            var controller = CreateController(ctx);

            var result = await controller.GetTripAnalytics(
                startDate: new DateTime(2025, 02, 01));

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TripAnalyticsDto>(ok.Value);

            Assert.Equal(1, dto.TotalTrips);
            Assert.Equal(20, dto.TotalRevenue);
            Assert.Equal(20, dto.AverageFare);
        }

        [Fact]
        public async Task GetTripAnalytics_AppliesEndDateFilter()
        {
            using var ctx = CreateContext();

            var zone = new TaxiZone { Zone = "TestZone" };
            var payment = new PaymentType { PaymentDescription = "Card" };

            ctx.TaxiZones.Add(zone);
            ctx.PaymentTypes.Add(payment);

            ctx.YellowTripData.AddRange(
                new YellowTripData
                {
                    PickupDatetime = new DateTime(2025, 01, 01),
                    TripDistance = 1.0m,
                    TotalAmount = 10,
                    PickupZone = zone,
                    PaymentType = payment
                },
                new YellowTripData
                {
                    PickupDatetime = new DateTime(2025, 03, 01),
                    TripDistance = 2.0m,
                    TotalAmount = 20,
                    PickupZone = zone,
                    PaymentType = payment
                }
            );

            await ctx.SaveChangesAsync();

            var controller = CreateController(ctx);

            var result = await controller.GetTripAnalytics(
                endDate: new DateTime(2025, 02, 01));

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TripAnalyticsDto>(ok.Value);

            Assert.Equal(1, dto.TotalTrips);
            Assert.Equal(10, dto.TotalRevenue);
            Assert.Equal(10, dto.AverageFare);
        }

        [Fact]
        public async Task GetTripAnalytics_ComputesAnalyticsCorrectly()
        {
            using var ctx = CreateContext();

            var zone1 = new TaxiZone { Zone = "ZoneA" };
            var zone2 = new TaxiZone { Zone = "ZoneB" };

            var pay1 = new PaymentType { PaymentDescription = "Cash" };
            var pay2 = new PaymentType { PaymentDescription = "Card" };

            ctx.TaxiZones.AddRange(zone1, zone2);
            ctx.PaymentTypes.AddRange(pay1, pay2);

            ctx.YellowTripData.AddRange(
                new YellowTripData
                {
                    PickupDatetime = DateTime.UtcNow,
                    TripDistance = 1,
                    TotalAmount = 10,
                    PickupZone = zone1,
                    PaymentType = pay1
                },
                new YellowTripData
                {
                    PickupDatetime = DateTime.UtcNow,
                    TripDistance = 3,
                    TotalAmount = 30,
                    PickupZone = zone1,
                    PaymentType = pay2
                },
                new YellowTripData
                {
                    PickupDatetime = DateTime.UtcNow,
                    TripDistance = 2,
                    TotalAmount = 20,
                    PickupZone = zone2,
                    PaymentType = pay1
                }
            );

            await ctx.SaveChangesAsync();

            var controller = CreateController(ctx);
            var result = await controller.GetTripAnalytics();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TripAnalyticsDto>(ok.Value);

            Assert.Equal(3, dto.TotalTrips);
            Assert.Equal(20, dto.AverageFare);        
            Assert.Equal(2, dto.AverageDistance);     
            Assert.Equal(60, dto.TotalRevenue);       
            Assert.Equal("ZoneA", dto.MostPopularPickupZone);  
            Assert.Equal("Cash", dto.MostUsedPaymentType);     
        }
    }
}
