using GreenBowl.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenBowl.Services
{
    public class InventoryCalculationService
    {
        private readonly AppDbContext _context;

        public InventoryCalculationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task RecalculateAsync(int lot)
        {
            var inventory = await _context.ProductInventories
                .FirstOrDefaultAsync(x => x.Lot == lot);

            if (inventory == null)
                return;

            var packaging = await _context.BatchBCPas
                .Where(x => x.Lot == lot)
                .ToListAsync();
            var batching = await _context.BatchBCBS
                .Where(x => x.Lot == lot)
                .ToListAsync();
            var sold = await _context.Receipts
                .Where(x => x.Lot == lot)
                .ToListAsync();
            var totalSold = sold.Sum(x => x.Quantity);

            inventory.Pouch = packaging.Sum(x => x.NumberOfPouchMade);
            inventory.BatchWeight = batching.Sum(x => x.AmountPerBatch);
            inventory.WeightRejected = packaging.Sum(x => x.WeightRejected);
            inventory.SealingRejected = packaging.Sum(x => x.SealingRejected);
            inventory.NoPrint = packaging.Sum(x => x.NoPrintRejected);

            inventory.XRayRejected = await _context.BatchBCPrs
                .Where(x => x.Lot == lot)
                .SumAsync(x => x.NumberOfRejectedPouch);

            inventory.TotalRejected =
                inventory.WeightRejected +
                inventory.XRayRejected +
                inventory.SealingRejected +
                inventory.NoPrint;

            inventory.TotalCase =
                inventory.Pouch + inventory.TotalRejected;

            inventory.ActualInventory =
                inventory.Pouch - inventory.Sample - inventory.QCRetention - totalSold;

            inventory.ProductForSale = inventory.ActualInventory;

            inventory.Productivity =
                inventory.TotalCase == 0
                    ? 0
                    : (int)Math.Round(
                        (double)inventory.Pouch / inventory.TotalCase * 100
                    );

            inventory.QACheck =
                await _context.BatchingControlPackagings.AnyAsync(x => x.Lot == lot)
                && await _context.BCPaChecks.AnyAsync(x => x.Lot == lot)
                && await _context.BatchingControlProcessings.AnyAsync(x => x.Lot == lot)
                && await _context.BCPrChecks.AnyAsync(x => x.Lot == lot)
                && await _context.BatchingControlEquipments.AnyAsync(x => x.Lot == lot)
                && await _context.BatchingControlBatchingSteps.AnyAsync(x => x.Lot == lot)
                && await _context.BatchBCBS.AnyAsync(x => x.Lot == lot)
                && await _context.XRayMonitoringRecords.AnyAsync(x => x.Lot == lot)
                && await _context.BulgingTestOnPouches.AnyAsync(x => x.Lot == lot);

            await _context.SaveChangesAsync();
        }
    }
}
