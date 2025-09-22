using FintcsApi.Data;

namespace FintcsApi.Services
{
    public class VoucherService
    {
        private readonly AppDbContext _context;

        public VoucherService(AppDbContext context)
        {
            _context = context;
        }

        // Later you can add CRUD methods for Voucher
    }
}
