using Microsoft.AspNetCore.Mvc;
using TaxiBoard.Controllers;
using TaxiBoard.Data;
using TaxiBoard.Models;
using TaxiBoard.Tests.TestUtilities;
using TaxiBoard.DTOs;
using Xunit;

namespace TaxiBoard.Tests.Controllers
{
    public class ZonesControllerTests
    {
        private TaxiBoardContext GetContext() => TestDbContextFactory.Create();

        [Fact]
        public async Task GetZones_ReturnsAllZones()
        {
            var context = GetContext();

            context.TaxiZones.Add(new TaxiZone
            {
                LocationId = 1,
                Borough = "Manhattan",
                Zone = "Upper East Side",
                ServiceZone = "Yellow"
            });

            context.TaxiZones.Add(new TaxiZone
            {
                LocationId = 2,
                Borough = "Queens",
                Zone = "Astoria",
                ServiceZone = "Green"
            });

            context.SaveChanges();

            var controller = new ZonesController(context);

            var result = await controller.GetZones();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<ZoneDto>>(ok.Value);

            Assert.Equal(2, data.Count());
        }

        [Fact]
        public async Task GetZones_FiltersByBorough()
        {
            var context = GetContext();

            context.TaxiZones.Add(new TaxiZone
            {
                LocationId = 1,
                Borough = "Manhattan",
                Zone = "Chelsea",
                ServiceZone = "Yellow"
            });

            context.TaxiZones.Add(new TaxiZone
            {
                LocationId = 2,
                Borough = "Queens",
                Zone = "Astoria",
                ServiceZone = "Green"
            });

            context.SaveChanges();

            var controller = new ZonesController(context);

            var result = await controller.GetZones("Manhattan");

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<ZoneDto>>(ok.Value);

            Assert.Single(data);
            Assert.Equal("Manhattan", data.First().Borough);
        }
    }
}
