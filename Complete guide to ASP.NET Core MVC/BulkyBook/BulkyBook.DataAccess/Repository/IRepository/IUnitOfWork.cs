namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }

        ICoverRepository CoverRepository { get; }   

        IProductRepository ProductRepository { get; }

        void Save();
    }
}
