using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface ICoverRepository : IRepository<Cover>
    {
        void Update(Cover cover);
    }
}