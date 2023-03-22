namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }

        ICoverRepository CoverRepository { get; }   

        IProductRepository ProductRepository { get; }

        ICompanyRepository CompanyRepository { get; }

        IShoppingCartRepository ShoppingCartRepository { get; }

        IApplicationUserRepository ApplicationUserRepository { get; }

        IOrderHeaderRepository OrderHeaderRepository { get; }

        IOrderDetailRepository OrderDetailRepository { get; }

        void Save();
    }
}