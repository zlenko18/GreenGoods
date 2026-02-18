using GreenBowl.Models;
using GreenBowl.Models.ViewModels;
using GreenBowl.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenBowl.Controllers
{
    public class IngredientsInventoryController : Controller
    {
        private readonly AppDbContext _context;

        public IngredientsInventoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /IngredientsInventory
        public async Task<IActionResult> Index()
        {
            try
            {
                var items = await _context.IngredientsInventories
                    .AsNoTracking()
                    .OrderByDescending(x => x.IngredientLot)
                    .ToListAsync();

                var trucks = await _context.InboundTrailers
                    .AsNoTracking()
                    .OrderByDescending(x => x.IngredientLot)
                    .ToListAsync();


                var vm = new IngredientsInventoryIndexVM
                {
                    items = items,
                    trucks = trucks
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Failed to load Ingredients Inventory. {ex.Message}";
                return View(new IngredientsInventoryIndexVM
                {
                    items = new List<IngredientsInventory>(),
                    trucks = new List<InboundTrailer>()
                });
            }
        }

        // GET: /IngredientsInventory/Create
        public IActionResult Create()
        {
            return View(new IngredientsInventoryInboundFormVM());
        }

        // POST: /IngredientsInventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IngredientsInventoryInboundFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Err"] = "Please fix validation errors and try again.";
                return View(vm);
            }

            try
            {
                vm.IngredientLot = await GenerateIngredientLotAsync(vm.IngredientName);

                var item = new IngredientsInventory
                {
                    IngredientLot = vm.IngredientLot,
                    IngredientName = vm.IngredientName,
                    Quantity = vm.Quantity
                };

                var truck = new InboundTrailer
                {
                    IngredientLot = vm.IngredientLot,
                    SupplierName = vm.SupplierName,
                    TruckPlate = vm.TruckPlate,
                    QualityCheck = vm.QualityCheck
                };

                _context.IngredientsInventories.Add(item);
                _context.InboundTrailers.Add(truck);

                await _context.SaveChangesAsync();

                TempData["Ok"] = $"Ingredient lot {vm.IngredientLot} created.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Err"] = $"Database rejected the save. {dbEx.InnerException?.Message ?? dbEx.Message}";
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Unexpected error while creating ingredient lot. {ex.Message}";
                return View(vm);
            }
        }

        // GET: /IngredientsInventory/Edit?ingredientLot=RI000001
        public async Task<IActionResult> Edit(string ingredientLot)
        {
            try
            {
                var item = await _context.IngredientsInventories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IngredientLot == ingredientLot);

                var truck = await _context.InboundTrailers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IngredientLot == ingredientLot);

                if (item == null || truck == null)
                {
                    TempData["Err"] = $"Ingredient lot {ingredientLot} not found.";
                    return RedirectToAction(nameof(Index));
                }

                var vm = new IngredientsInventoryInboundFormVM
                {
                    IngredientLot = item.IngredientLot,
                    IngredientName = item.IngredientName,
                    Quantity = item.Quantity,
                    SupplierName = truck.SupplierName,
                    TruckPlate = truck.TruckPlate,
                    QualityCheck = truck.QualityCheck
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Failed to load ingredient lot {ingredientLot}. {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /IngredientsInventory/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IngredientsInventoryInboundFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Err"] = "Please fix validation errors and try again.";
                return View(vm);
            }

            try
            {
                var item = await _context.IngredientsInventories
                    .FirstOrDefaultAsync(x => x.IngredientLot == vm.IngredientLot);

                var truck = await _context.InboundTrailers
                    .FirstOrDefaultAsync(x => x.IngredientLot == vm.IngredientLot);

                if (item == null || truck == null)
                {
                    TempData["Err"] = $"Ingredient lot {vm.IngredientLot} not found.";
                    return RedirectToAction(nameof(Index));
                }

                item.IngredientName = vm.IngredientName;
                item.Quantity = vm.Quantity;

                truck.SupplierName = vm.SupplierName;
                truck.TruckPlate = vm.TruckPlate;
                truck.QualityCheck = vm.QualityCheck;

                await _context.SaveChangesAsync();

                TempData["Ok"] = $"Ingredient lot {vm.IngredientLot} updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Err"] = $"Database rejected the save. {dbEx.InnerException?.Message ?? dbEx.Message}";
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Unexpected error while saving. {ex.Message}";
                return View(vm);
            }
        }

        private async Task<string> GenerateIngredientLotAsync(IngredientName ingredient)
        {
            var prefix = ingredient.ToString().ToUpperInvariant();
            prefix = prefix.Length >= 2 ? prefix.Substring(0, 2) : (prefix + "X");

            var maxExisting = await _context.IngredientsInventories
                .Where(x => x.IngredientLot.StartsWith(prefix))
                .Select(x => x.IngredientLot)
                .ToListAsync();

            var maxNum = 0;
            foreach (var lot in maxExisting)
            {
                var tail = lot.Length > 2 ? lot.Substring(2) : "";
                if (int.TryParse(tail, out var n) && n > maxNum)
                    maxNum = n;
            }

            var next = maxNum + 1;
            return $"{prefix}{next:D6}";
        }
    }
}
