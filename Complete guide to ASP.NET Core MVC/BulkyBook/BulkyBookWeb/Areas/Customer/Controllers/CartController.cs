using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBook.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
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
                ListCart = _unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new(),
            };

            Model.OrderHeader.OrderTotal = 0;
            foreach (var cart in Model.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                Model.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }

            return View(Model);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            Model = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new(),
            };

            Model.OrderHeader.AplicationUser = _unitOfWork.ApplicationUserRepository.GetFirstOrDefault(x => x.Id == claim.Value);

            Model.OrderHeader.Name = Model.OrderHeader.AplicationUser.Name;
            Model.OrderHeader.PhoneNumber = Model.OrderHeader.AplicationUser.PhoneNumber;
            Model.OrderHeader.StreetAddress = Model.OrderHeader.AplicationUser.StreetAddress;
            Model.OrderHeader.City = Model.OrderHeader.AplicationUser.City;
            Model.OrderHeader.State = Model.OrderHeader.AplicationUser.State;
            Model.OrderHeader.PostalCode = Model.OrderHeader.AplicationUser.PostalCode;
            
            Model.OrderHeader.OrderTotal = 0;
            foreach (var cart in Model.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                Model.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }

            return View(Model);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            Model.ListCart = _unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product");

            Model.OrderHeader.OrderDate = DateTime.Now;
            Model.OrderHeader.ApplicationUserId = claim.Value;

            // This is neded because of the '+=' usage. Idk sometimes it worked without it.
            Model.OrderHeader.OrderTotal = 0;
            foreach (var cart in Model.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                Model.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }

            ApplicationUser applicationUser = _unitOfWork.ApplicationUserRepository.GetFirstOrDefault(x => x.Id == claim.Value);
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                Model.OrderHeader.PaymentStatuss = SD.PaymentStatussPending;
                Model.OrderHeader.OrderStatuss = SD.StatussPending;
            }
            else
            {
                Model.OrderHeader.PaymentStatuss = SD.PaymentStatussDelayedPayment;
                Model.OrderHeader.OrderStatuss = SD.StatussApproved;
            }

            _unitOfWork.OrderHeaderRepository.Add(Model.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in Model.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = Model.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };

                _unitOfWork.OrderDetailRepository.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:44382/";

                // Stripe settings
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={Model.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                };

                foreach (var item in Model.ListCart)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)item.Price * 100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title,
                            },
                        },
                        Quantity = item.Count,
                    };

                    options.LineItems.Add(sessionLineItem);
                };

                var service = new SessionService();
                Session session = service.Create(options);
                Model.OrderHeader.SessionId = session.Id;
                Model.OrderHeader.PaymentIntentId = session.PaymentIntentId;
                _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(Model.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            else
            {
                return RedirectToAction("OrderConfirmation", "Cart", new { id = Model.OrderHeader.Id});
            }
        }

        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == id);

            if (orderHeader.PaymentStatuss != SD.PaymentStatussDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaderRepository.UpdateStatuss(id, SD.StatussApproved, SD.PaymentStatussApproved);
                    _unitOfWork.Save();
                }
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCartRepository.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            return View(id);
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
