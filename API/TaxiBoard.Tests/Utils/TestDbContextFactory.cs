using Microsoft.EntityFrameworkCore;
using TaxiBoard.Data;


namespace TaxiBoard.Tests.TestUtilities
{
    public static class TestDbContextFactory
    {
        public static TaxiBoardContext Create()
        {
            var options = new DbContextOptionsBuilder<TaxiBoardContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TaxiBoardContext(options);
        }
    }
}
