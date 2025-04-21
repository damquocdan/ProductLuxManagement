using OfficeFurnitureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace OfficeFurnitureStore.Areas.AdminStore.Controllers {

    public class DashboardController : BaseController
    {
        private readonly OfficeFurnitureStoreContext _context;

        public DashboardController(OfficeFurnitureStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }
    }

}
