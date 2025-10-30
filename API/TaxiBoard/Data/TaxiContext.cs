using Microsoft.EntityFrameworkCore;
// using TaxiBoard.Models; add this later
namespace TaxiBoard.Data
{
    public class TaxiBoardContext : DbContext
    {
        public TaxiBoardContext(DbContextOptions<TaxiBoardContext> options)
            : base(options)
        {
        }

        // Add DB sets here later
    }
}
