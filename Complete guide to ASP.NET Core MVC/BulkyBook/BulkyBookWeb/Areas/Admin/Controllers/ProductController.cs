using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, 
                                            IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var _productList = _unitOfWork.ProductRepository.GetAll();
            return View(_productList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductViewModel productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.CategoryRepository
                    .GetAll()
                    .Select(
                            x => new SelectListItem
                            {
                                Text = x.Name,
                                Value = x.Id.ToString()
                            }),
                CoverTypeList = _unitOfWork.CoverRepository
                    .GetAll()
                    .Select(
                            x => new SelectListItem
                            {
                                Text = x.Name,
                                Value = x.Id.ToString()
                            }),
            };

            if (id is null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                // Updates productVM
            }

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var rootPath = _webHostEnvironment.WebRootPath;
                
                // I think this is redundant, because the frontent handles this
                if (file is not null)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var upliads = Path.Combine(rootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(upliads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    productVM.Product.ImageUrl = $@"\images\products\{fileName}{extension}";
                }
                _unitOfWork.ProductRepository.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully!";
                return RedirectToAction("Index");
            }

            return View(productVM);
        }

        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }

            var product = _unitOfWork.ProductRepository.GetFirstOrDefault(x => x.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            _unitOfWork.ProductRepository.Remove(product);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
