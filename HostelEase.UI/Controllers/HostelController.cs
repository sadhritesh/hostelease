using HostelEase.Application.Interfaces.ServiceContracts;
using HostelEase.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HostelEase.UI.Controllers
{
    [Route("[controller]")]
    public class HostelController : Controller
    {
        private readonly IHostelService _hostelService;

        public HostelController(IHostelService hostelService)
        {
            _hostelService = hostelService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var hostels = await _hostelService.GetAllHostels();
            return View(hostels);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Hostel hostel)
        {
            await _hostelService.AddHostel(hostel);
            return RedirectToAction("Index");
        }
    }
}
