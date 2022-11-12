using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class CoverRepository : Repository<Cover>, ICoverRepository
    {
        private readonly ApplicationDbContext _db;

        public CoverRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Cover cover)
        {
            _db.Covers.Update(cover);
        }
    }
}
