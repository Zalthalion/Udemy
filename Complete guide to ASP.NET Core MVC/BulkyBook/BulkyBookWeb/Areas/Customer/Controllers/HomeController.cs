using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBook.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category,Cover");
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cartObj = new()
            {
                Product = _unitOfWork.ProductRepository.GetFirstOrDefault(x => x.Id == productId, includeProperties: "Category,Cover"),
                ProductId = productId,
                Count = 1,
            };
                
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            var cartFromDb = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(
                x => x.ApplicationUserId == claim.Value 
                && x.ProductId == shoppingCart.ProductId);

            if (cartFromDb is null)
            {
                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.IncrementCount(cartFromDb, shoppingCart.Count);
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}