using Hotel.Application.Common.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Hotel.Infrastructue.Data;
using Hotel.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Hotel.Web.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _VillaPath;

        public AmenityController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _VillaPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "RoomImages", "Villa");
        }

        public IActionResult Index()
        {
            var amenities = _unitOfWork.Amenity.GetAll(includeProperties: "villa");
            if (amenities == null)
            {
                return View(new List<Amenity>());
            }
            return View(amenities);
        }

        public IActionResult Create()
        {
            AmenityVM VillaList = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToList()
            };

            return View(VillaList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AmenityVM obj)
        {
            if (ModelState.IsValid )
            {

                _unitOfWork.Amenity.Add(obj.Amenity);
                _unitOfWork.Save();
                TempData["Success"] = "Amenity  Has Been Added Successfully";

                return RedirectToAction(nameof(Index));
            }

            obj.VillaList = _unitOfWork.Villa.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();

            return View(obj);
        }
        public IActionResult Update(int id)
        {
            var selectedAmenity = _unitOfWork.Amenity.Get(x => x.Id == id);
            if (selectedAmenity == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var AmenityVm = new AmenityVM
            {
                Amenity = selectedAmenity,
                VillaList = _unitOfWork.Villa.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            return View(AmenityVm);
        }

        [HttpPost]
        public IActionResult Update(AmenityVM amenitiyVM)
        {

            if (ModelState.IsValid)
            {

                _unitOfWork.Amenity.Update(amenitiyVM.Amenity);
                _unitOfWork.Save();
                TempData["Success"] = "Amenity Number Has Been Updated Successfully";

                return RedirectToAction(nameof(Index));
            }

            amenitiyVM.VillaList = _unitOfWork.Villa.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();
            return View(amenitiyVM);
        }



        public IActionResult Delete(int id)
        {
            var AmeityToBeDe = _unitOfWork.Amenity.Get(x => x.Id == id);
            if (AmeityToBeDe == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var AmenityVM = new AmenityVM
            {
                Amenity = AmeityToBeDe,
                VillaList = _unitOfWork.Villa.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            return View(AmenityVM);
        }
        [HttpPost]
        public IActionResult Delete(AmenityVM obj)
        {
            var AmeityToBeDe = _unitOfWork.Amenity.Get(x => x.Id == obj.Amenity.Id);

            if (AmeityToBeDe is not null)
            {
                _unitOfWork.Amenity.Remove(AmeityToBeDe);
                _unitOfWork.Save();
                TempData["Success"] = "Amenity Number Has Benn Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }
    }
}
