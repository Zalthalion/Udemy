﻿using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            CategoryRepository = new CategoryRepository(_db);
            CoverRepository = new CoverRepository(_db); 
            ProductRepository = new ProductRepository(_db);
        }

        public ICategoryRepository CategoryRepository { get; private set; }

        public ICoverRepository CoverRepository { get; private set; }

        public IProductRepository  ProductRepository { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}