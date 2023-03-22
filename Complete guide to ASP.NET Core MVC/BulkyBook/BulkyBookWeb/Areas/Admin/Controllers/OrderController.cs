using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM Model { get; set; }

		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Details(int id)
        {
            Model = new()
            {
                OrderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == id, includeProperties: "AplicationUser"),
                OrderDetails = _unitOfWork.OrderDetailRepository.GetAll(x => x.Id == id, includeProperties: "Product"),
            };

            return View(Model);
        }

        [ActionName("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPay()
        {
            Model.OrderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == Model.OrderHeader.Id, includeProperties: "AplicationUser");
            Model.OrderDetails = _unitOfWork.OrderDetailRepository.GetAll(x => x.OrderId == Model.OrderHeader.Id, includeProperties: "Product");

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
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?id={Model.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?id={Model.OrderHeader.Id}",
            };

            foreach (var item in Model.OrderDetails)
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

        public IActionResult PaymentConfirmation(int id)
        {
            var orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == id);

            if (orderHeader.PaymentStatuss == SD.PaymentStatussDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaderRepository.UpdateStatuss(id, orderHeader.OrderStatuss, SD.PaymentStatussApproved);
                    _unitOfWork.Save();
                }
            }

            return View(id);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            var orderFromDb = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == Model.OrderHeader.Id);
            orderFromDb.Name = Model.OrderHeader.Name;
            orderFromDb.PhoneNumber = Model.OrderHeader.Name;
            orderFromDb.StreetAddress = Model.OrderHeader.Name;
            orderFromDb.City = Model.OrderHeader.Name;
            orderFromDb.State = Model.OrderHeader.Name;
            orderFromDb.PostalCode = Model.OrderHeader.Name;

            if (Model.OrderHeader.Carrier is not null)
            {
                orderFromDb.Carrier = Model.OrderHeader.Carrier;
            }

            if (Model.OrderHeader.TrackingNumber is not null)
            {
                orderFromDb.TrackingNumber = Model.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeaderRepository.Update(orderFromDb);
            _unitOfWork.Save();

            return RedirectToAction("Details", "Order", new { id = orderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeaderRepository.UpdateStatuss(Model.OrderHeader.Id, SD.StatussInProcess);
            _unitOfWork.Save();

            return RedirectToAction("Details", "Order", new { id = Model.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder()
        {
            var orderFromDb = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == Model.OrderHeader.Id);
            orderFromDb.TrackingNumber = Model.OrderHeader.TrackingNumber;
            orderFromDb.Carrier = Model.OrderHeader.Carrier;
            orderFromDb.OrderStatuss = SD.StatussShipped;
            orderFromDb.ShippingDate = DateTime.Now;

            if (orderFromDb.PaymentStatuss == SD.PaymentStatussDelayedPayment)
            {
                orderFromDb.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.OrderHeaderRepository.Update(orderFromDb);
            _unitOfWork.Save();

            return RedirectToAction("Details", "Order", new { id = Model.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            var orderFromDb = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == Model.OrderHeader.Id);
            
            if (orderFromDb.PaymentStatuss == SD.PaymentStatussApproved)
            {
                // Needs to refund
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFromDb.PaymentIntentId,
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeaderRepository.UpdateStatuss(orderFromDb.Id, SD.StatussCancelled, SD.Statuss_name);
            }
            else
            {
                // Does not need a refund
                _unitOfWork.OrderHeaderRepository.UpdateStatuss(orderFromDb.Id, SD.StatussCancelled, SD.StatussCancelled);
            }

            _unitOfWork.Save();

            return RedirectToAction("Details", "Order", new { id = Model.OrderHeader.Id });
        }

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "AplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "AplicationUser");
            }


            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatuss == SD.PaymentStatussDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatuss == SD.StatussInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatuss == SD.StatussShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatuss == SD.StatussApproved);
                    break;
            }

            return Json(new { data = orderHeaders });
        }
    }
}
