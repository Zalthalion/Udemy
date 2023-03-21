using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatuss(int id, string orderStatuss, string? paymentStatus = null)
        {
            var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);

            if (orderHeaderFromDb is not null)
            {
                orderHeaderFromDb.OrderStatuss = orderStatuss;
                if (paymentStatus is not null)
                {
                    orderHeaderFromDb.PaymentStatuss = paymentStatus;
                }
            }
        }
    }
}