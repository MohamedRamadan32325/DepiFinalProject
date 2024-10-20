﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;
using WebApplication7.Repositry.IRepositry;
using WebApplication7.ViewModels;

namespace WebApplication7.Controllers
{
    public class PlaceController : Controller
    {
        private readonly IPlace _placeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;



        public PlaceController(IPlace placeRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> _signInManager, IWebHostEnvironment webHostEnvironment)



        {
            _userManager = userManager;
            signInManager = _signInManager;
            _webHostEnvironment = webHostEnvironment;
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
            var newplace = _placeRepository.GetByName(place.Place_Name);
            if ( newplace!=null && newplace.Place_Type==place.Place_Type)
            {
                ModelState.AddModelError("Place_Name", "This Place is Already Exist");
            }
            if (ModelState.IsValid)
            {
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
        public IActionResult SaveEdit(PlaceViewModel updatedPlace)
        {
            if (ModelState.IsValid)
            {
                var existingPlace = _placeRepository.Get(updatedPlace.SpecificPlace.Place_Id);
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
