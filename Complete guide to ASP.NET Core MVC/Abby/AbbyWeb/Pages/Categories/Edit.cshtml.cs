using AbbyWeb.Data;
using AbbyWeb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbbyWeb.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Category Category { get; set; }

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet(int id)
        {
            Category = _db.Categories.FirstOrDefault(x => x.Id == id);
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                if (Category is null)
                {
                    return NotFound();
                }

                _db.Update(Category);
                _db.SaveChanges();
                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}
