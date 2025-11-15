using Microsoft.AspNetCore.Mvc;
using TaxiBoard.Controllers;
using TaxiBoard.Data;
using TaxiBoard.Models;
using TaxiBoard.Tests.TestUtilities;
using TaxiBoard.DTOs;
using Xunit;

namespace TaxiBoard.Tests.Controllers
{
    public class VendorsControllerTests
    {
        private TaxiBoardContext GetContext() => TestDbContextFactory.Create();

        [Fact]
        public async Task GetVendors_ReturnsAllVendors()
        {
            var context = GetContext();

            context.Vendors.Add(new Vendor
            {
                VendorId = 1,
                VendorName = "V1"
            });

            context.Vendors.Add(new Vendor
            {
                VendorId = 2,
                VendorName = "V2"
            });

            context.SaveChanges();

            var controller = new VendorsController(context);

            var result = await controller.GetVendors();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<VendorDto>>(ok.Value);

            Assert.Equal(2, data.Count());
            Assert.Contains(data, v => v.VendorName == "V1");
            Assert.Contains(data, v => v.VendorName == "V2");
        }
    }
}
