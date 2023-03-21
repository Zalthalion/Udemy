using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Security.Claims;

namespace BulkyBook.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartVM Model { get; set; }

        public int OrderTotal { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            Model = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            foreach (var cart in Model.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                Model.CartTotal += cart.Price * cart.Count;
            }

            return View(Model);
        }

        public IActionResult Summary()
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //Model = new ShoppingCartVM()
            //{
            //    ListCart = _unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product")
            //};

            //foreach (var cart in Model.ListCart)
            //{
            //    cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
            //    Model.CartTotal += cart.Price * cart.Count;
            //}

            return View();
        }

        // I dont like that this refreshes the page everytime, there probably is a better way to do it
        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
            _unitOfWork.ShoppingCartRepository.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCartRepository.Remove(cart);
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.DecrementCount(cart, 1);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
            _unitOfWork.ShoppingCartRepository.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else
            {
                if (quantity <= 100)
                {
                    return price50;
                }
                else
                {
                    return price100;
                }
            }
        }
    }
}
