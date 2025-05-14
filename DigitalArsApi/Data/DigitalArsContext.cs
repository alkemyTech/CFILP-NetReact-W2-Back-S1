using Microsoft.EntityFrameworkCore;

namespace DigitalArsApi.Data
{
    public class DigitalArsContext : DbContext
    {
        public DigitalArsContext(DbContextOptions<DigitalArsContext> options)
            : base(options) {}
    }
}
