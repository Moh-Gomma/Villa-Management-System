using Hotel.Application.Common.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Hotel.Infrastructue.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Hotel.Web.Controllers
{

    [Authorize(Roles =SD.Role_Admin)]
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _VillaPath;

        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _VillaPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "RoomImages", "Villa");
        }

        public IActionResult Index()
        {
            var villas = _unitOfWork.Villa.GetAll();
            return View(villas);
        }

        private async Task<string> HandleImageUpload(IFormFile image , string oldImage = null)
        {
            if (!Directory.Exists(_VillaPath))
            {
                Directory.CreateDirectory(_VillaPath);
            }
            if(image == null)
            {
                return oldImage ?? "https://placehold.co/600x400/EEE/31343C";
            }


            if (!string.IsNullOrEmpty(oldImage))
            {
                var oldImageFullPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImage.TrimStart('\\'));
                if (System.IO.File.Exists(oldImageFullPath))
                {
                    System.IO.File.Delete(oldImageFullPath);
                }
            }

            // Save new image
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(_VillaPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream); 
            }

            return $"/Images/RoomImages/Villa/{fileName}";
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Create(Villa obj)
        {
            if(obj.Name == obj.Description)
            {
                ModelState.AddModelError("Description", "The  Description Can't be the same of Name");
            }
            if (ModelState.IsValid)
            {
                obj.ImageUrl = await HandleImageUpload(obj.Image);
                _unitOfWork.Villa.Add(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Villa Has Been Added Successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }
        public IActionResult Update(int id)
        {
            var obj = _unitOfWork.Villa.Get(u => u.Id == id);
            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public async Task< IActionResult> Update(Villa obj)
        {

            if (ModelState.IsValid)
            {
                obj.ImageUrl = await HandleImageUpload(obj.Image, obj.ImageUrl);
                _unitOfWork.Villa.Update(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Villa Has Been Updated Successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }



        public IActionResult Delete(int id)
        {
            var obj = _unitOfWork.Villa.Get(u => u.Id == id);
            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Delete(Villa obj)
        {

            if (obj is not null)
            {
                _unitOfWork.Villa.Remove(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Villa Has Benn Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }
    }
}
