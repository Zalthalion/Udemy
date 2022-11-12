using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var _coverList = _unitOfWork.CoverRepository.GetAll();
            return View(_coverList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cover cover)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverRepository.Add(cover);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(cover);
        }

        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }

            var cover = _unitOfWork.CoverRepository.GetFirstOrDefault(x => x.Id == id); ;

            if (cover is null)
            {
                return NotFound();
            }

            return View(cover);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cover cover)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverRepository.Update(cover);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(cover);
        }

        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }

            var cover = _unitOfWork.CoverRepository.GetFirstOrDefault(x => x.Id == id);

            if (cover is null)
            {
                return NotFound();
            }

            _unitOfWork.CoverRepository.Remove(cover);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
