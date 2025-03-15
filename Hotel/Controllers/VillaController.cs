using Hotel.Application.Common.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Hotel.Infrastructue.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace Hotel.Web.Controllers
{

    [Authorize(Roles =SD.Role_Admin)]
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _VillaPath;
        private readonly ILogger<VillaController> _logger;
        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, ILogger<VillaController> logger)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _VillaPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "RoomImages", "Villa");
            _logger = logger;
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
                _logger.LogInformation($"Creating directory: {_VillaPath}");
                Directory.CreateDirectory(_VillaPath);
            }
            if(image == null)
            {
                _logger.LogWarning("No image file uploaded. Using oldImage or default.");
                return oldImage ?? "https://placehold.co/600x400/EEE/31343C";
            }


            if (!string.IsNullOrEmpty(oldImage))
            {
                var oldImageFullPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImage.TrimStart('\\'));
                if (System.IO.File.Exists(oldImageFullPath))
                {
                    _logger.LogInformation($"Deleting old image: {oldImageFullPath}");
                    System.IO.File.Delete(oldImageFullPath);
                }
            }

            // Save new image
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(_VillaPath, fileName);

            try
            {
                _logger.LogInformation($"Saving image to: {filePath}");
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                return $"/Images/RoomImages/Villa/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save image to: {filePath}", filePath);
                throw; 
            }
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Create(Villa obj, IFormFile? Image)
        {
            if(obj.Name == obj.Description)
            {
                ModelState.AddModelError("Description", "The  Description Can't be the same of Name");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    obj.ImageUrl = await HandleImageUpload(Image);
                    obj.CreatedDate = DateTime.Now;
                    _unitOfWork.Villa.Add(obj);
                    _unitOfWork.Save();
                    TempData["Success"] = "Villa Has Been Added Successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating villa with name: {Name}", obj.Name);
                    ModelState.AddModelError("", "An error occurred while uploading the image. Please try again.");
                }
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
        public async Task< IActionResult> Update(Villa obj, IFormFile? Image)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    obj.ImageUrl = await HandleImageUpload(Image, obj.ImageUrl);
                    obj.UpdatedDate = DateTime.Now;
                    _unitOfWork.Villa.Update(obj);
                    _unitOfWork.Save();
                    TempData["Success"] = "Villa Has Been Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating villa with ID: {Id}", obj.Id);
                    ModelState.AddModelError("", "An error occurred while uploading the image. Please try again.");
                }
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

            if (obj != null)
            {
                if (!string.IsNullOrEmpty(obj.ImageUrl) && obj.ImageUrl != "https://placehold.co/600x400/EEE/31343C")
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        _logger.LogInformation($"Deleting image for villa {obj.Id}: {imagePath}");
                        System.IO.File.Delete(imagePath);
                    }
                }
                _unitOfWork.Villa.Remove(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Villa Has Been Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }
    }
}
