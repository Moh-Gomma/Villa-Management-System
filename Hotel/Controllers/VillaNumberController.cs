using Hotel.Application.Common.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Hotel.Infrastructue.Data;
using Hotel.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Hotel.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]

    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties:"villa");
            return View(villaNumbers);
        }

        public IActionResult Create()
        {
            VillaNumberVM VillaList = new()
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
        public IActionResult Create(VillaNumberVM obj)
        {
            var ExistedVillaId = _unitOfWork.VillaNumber.Any(x => x.Villa_Number == obj.VillaNumber.Villa_Number);
            if (ModelState.IsValid && !ExistedVillaId)
            {

                _unitOfWork.VillaNumber.Add(obj.VillaNumber);
                _unitOfWork.Save();
                TempData["Success"] = "Villa Number Has Been Added Successfully";

                return RedirectToAction(nameof(Index));
            }
            if (ExistedVillaId)
            {
                TempData["Error"] = "Number is Already Exist";
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
            var selectedVilla = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == id);
            if (selectedVilla == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var villNumberVm = new VillaNumberVM
            {
                VillaNumber = selectedVilla,
                VillaList = _unitOfWork.Villa.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            return View(villNumberVm);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM villanumberVm)
        {

            if (ModelState.IsValid)
            {

                _unitOfWork.VillaNumber.Update(villanumberVm.VillaNumber);
                _unitOfWork.Save();
                TempData["Success"] = "Villa Number Has Been Updated Successfully";

                return RedirectToAction(nameof(Index));
            }

            villanumberVm.VillaList = _unitOfWork.Villa.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();
            return View(villanumberVm);
        }



        public IActionResult Delete(int id)
        {
            var selectedVilla = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == id);
            if (selectedVilla == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var villNumberVm = new VillaNumberVM
            {
                VillaNumber = selectedVilla,
                VillaList = _unitOfWork.Villa.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            return View(villNumberVm);
        }
        [HttpPost]
        public IActionResult Delete(VillaNumberVM obj)
        {
            var selectedVilla = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == obj.VillaNumber.Villa_Number);

            if (selectedVilla is not null)
            {
                _unitOfWork.VillaNumber.Remove(selectedVilla);
                _unitOfWork.Save();
                TempData["Success"] = "Villa Number Has Benn Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }
    }
}
