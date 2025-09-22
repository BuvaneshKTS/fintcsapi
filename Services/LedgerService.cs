using FintcsApi.Data;

namespace FintcsApi.Services
{
    public class LedgerService
    {
        private readonly AppDbContext _context;

        public LedgerService(AppDbContext context)
        {
            _context = context;
        }

        // Later you can add CRUD methods for Ledger
    }
}
