using GreenBowl.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenBowl.Controllers
{
    public abstract class FormControllerBase : Controller
    {
        protected readonly AppDbContext _context;

        protected FormControllerBase(AppDbContext context)
        {
            _context = context;
        }

        protected Task<bool> LotExistsAsync(int lot)
            => _context.ProductInventories.AnyAsync(x => x.Lot == lot);

        protected IActionResult Fail(Func<IActionResult> failureReturn, string failureMessage, Exception ex)
        {
            TempData["Error"] = failureMessage;

            if (ex is DbUpdateException dbEx)
                ModelState.AddModelError("", dbEx.InnerException?.Message ?? dbEx.Message);
            else
                ModelState.AddModelError("", ex.Message);

            return failureReturn();
        }

        protected async Task<IActionResult> SaveOrShowErrorAsync<VM>(
            VM vm,
            string successMessage, 
            string failureMessage,
            Func<IActionResult> successRedirect,
            Func<IActionResult> failureReturn,
            Func<Task> mutate)
        {
            if (!ModelState.IsValid)
                return failureReturn();

            try
            {
                await mutate();

                
                if (!ModelState.IsValid)
                    return failureReturn();

                await _context.SaveChangesAsync();

                TempData["Success"] = successMessage;
                return successRedirect();
            }
            catch (Exception ex)
            {
                return Fail(failureReturn, failureMessage, ex);
            }
        }
    }
}
