﻿using WebApplication7.Models;
using WebApplication7.Repositry.IRepositry;
using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;
using WebApplication7.Repositry.IRepositry;

namespace DEPI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IPlace _placeRepository;

        public AdminController(IPlace placeRepository)
        {
            _placeRepository = placeRepository;
        }

        public IActionResult Index()
        {
            var placesList = _placeRepository.GetAll();
            return View(placesList);
        }

        [HttpGet]
        public IActionResult AddPlace()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePlace(Place place)
        {
            if (ModelState.IsValid)
            {
                //if (place.ImageFile != null)
                //{ 
                //    string filePath = _upload.UploadFile("\\Images\\Place_Photo\\", place.ImageFile);
                //    if (!string.IsNullOrEmpty(filePath))
                //    {
                //        place.Place_Photo = filePath;
                //    }
                //    else
                //    {
                //        ModelState.AddModelError("", "File upload failed");
                //        return View("AddPlace", place);
                //    }
                //}
                if (place.clientFile != null)
                {
                    MemoryStream stream = new MemoryStream();
                    place.clientFile.CopyTo(stream);
                    place.dbimage = stream.ToArray();
                }

                _placeRepository.Add(place);
                return RedirectToAction(nameof(Index));
            }

            return View("AddPlace", place);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var place = _placeRepository.Get(id);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var place = _placeRepository.Get(id);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveEdit(Place updatedPlace)
        {
            if (ModelState.IsValid)
            {
                var existingPlace = _placeRepository.Get(updatedPlace.Place_Id);
                if (existingPlace == null)
                {
                    return NotFound();
                }
                _placeRepository.Edit(updatedPlace);
                return RedirectToAction(nameof(Index));
            }

            return View("Edit", updatedPlace);
        }
        public IActionResult Delete(int id)
        {
            _placeRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}