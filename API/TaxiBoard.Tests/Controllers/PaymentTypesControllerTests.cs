using Microsoft.AspNetCore.Mvc;
using TaxiBoard.Controllers;
using TaxiBoard.Data;
using TaxiBoard.Models;
using TaxiBoard.Tests.TestUtilities;
using TaxiBoard.DTOs;
using Xunit;

namespace TaxiBoard.Tests.Controllers
{
    public class PaymentTypesControllerTests
    {
        private TaxiBoardContext GetContext() => TestDbContextFactory.Create();

        [Fact]
        public async Task GetPaymentTypes_ReturnsAllPaymentTypes()
        {
            var context = GetContext();

            context.PaymentTypes.Add(new PaymentType
            {
                PaymentTypeId = 1,
                PaymentDescription = "Cash"
            });

            context.PaymentTypes.Add(new PaymentType
            {
                PaymentTypeId = 2,
                PaymentDescription = "Card"
            });

            context.SaveChanges();

            var controller = new PaymentTypesController(context);

            var result = await controller.GetPaymentTypes();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<PaymentTypeDto>>(ok.Value);

            Assert.Equal(2, data.Count());
            Assert.Contains(data, p => p.PaymentDescription == "Cash");
            Assert.Contains(data, p => p.PaymentDescription == "Card");
        }
    }
}
