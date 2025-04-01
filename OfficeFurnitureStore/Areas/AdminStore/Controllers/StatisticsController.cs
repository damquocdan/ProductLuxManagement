using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeFurnitureStore.Models;

namespace OfficeFurnitureStore.Areas.AdminStore.Controllers
{
    [Area("AdminStore")]
    public class StatisticsController : Controller
    {
        private readonly OfficeFurnitureStoreContext _context;

        public StatisticsController(OfficeFurnitureStoreContext context)
        {
            _context = context;
        }

        // GET: AdminStore/Statistics
        public async Task<IActionResult> Index()
        {
            return View(await _context.Statistics.ToListAsync());
        }

        // GET: AdminStore/Statistics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistics
                .FirstOrDefaultAsync(m => m.StatisticId == id);
            if (statistic == null)
            {
                return NotFound();
            }

            return View(statistic);
        }

        // GET: AdminStore/Statistics/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminStore/Statistics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatisticId,ReportDate,Period,PeriodStartDate,PeriodEndDate,TotalOrders,TotalRevenue,TotalProductsSold,TotalCustomers,TotalProductsInStock,CategoryRevenue,ProductRevenue,CustomerRevenue")] Statistic statistic)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statistic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(statistic);
        }

        // GET: AdminStore/Statistics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistics.FindAsync(id);
            if (statistic == null)
            {
                return NotFound();
            }
            return View(statistic);
        }

        // POST: AdminStore/Statistics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StatisticId,ReportDate,Period,PeriodStartDate,PeriodEndDate,TotalOrders,TotalRevenue,TotalProductsSold,TotalCustomers,TotalProductsInStock,CategoryRevenue,ProductRevenue,CustomerRevenue")] Statistic statistic)
        {
            if (id != statistic.StatisticId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statistic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatisticExists(statistic.StatisticId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(statistic);
        }

        // GET: AdminStore/Statistics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistics
                .FirstOrDefaultAsync(m => m.StatisticId == id);
            if (statistic == null)
            {
                return NotFound();
            }

            return View(statistic);
        }

        // POST: AdminStore/Statistics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var statistic = await _context.Statistics.FindAsync(id);
            if (statistic != null)
            {
                _context.Statistics.Remove(statistic);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatisticExists(int id)
        {
            return _context.Statistics.Any(e => e.StatisticId == id);
        }
    }
}
