﻿using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;
using WebApplication7.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using WebApplication7.Repositry.IRepositry;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Identity;
using WebApplication7.Repositry;

public class BookingController : Controller
{
    private readonly IPlace _placeRepository;
    private readonly IBooking _bookingRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IWebHostEnvironment _webHostEnvironment;



    public BookingController(IPlace placeRepository,IBooking bookingRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> _signInManager, IWebHostEnvironment webHostEnvironment)



    {
        _userManager = userManager;
        signInManager = _signInManager;
        _webHostEnvironment = webHostEnvironment;
        _placeRepository = placeRepository;
        _bookingRepository = bookingRepository;

    }

    [HttpGet]
    public IActionResult Create(int id)
    {
        var bookingViewModel = _bookingRepository.Create(id);

        if (bookingViewModel == null)
        {
            return NotFound("Place not found");
        }

        return View(bookingViewModel);
    }

    [HttpPost]
    public IActionResult Create(BookingViewModel bookingViewModel,string promotioncode)
    {
        if (ModelState.IsValid)
        {
            return RedirectToAction("Payment", new { id = bookingViewModel.PlaceID, totalAmount = bookingViewModel.TotalAmount });
        }

        return View(bookingViewModel);
    }

   
[HttpGet]
    public IActionResult Payment(int id, int numberofguests, int dayes, string PromotionCode)
    {
        if (!User.Identity.IsAuthenticated)
        {
            TempData["ErrorMessage"] = "Please Login First";
            return RedirectToAction("Login", "Account");
        }

        var paymentViewModel = _bookingRepository.Payment(id, numberofguests, dayes, PromotionCode);
        var placeviewmodel = _placeRepository.Get(id);

        if (paymentViewModel == null)
        {
            return NotFound("Place not found");
        }

        if (!ModelState.IsValid)
        {
            BookingViewModel bookingViewModel = new BookingViewModel
            {
                PlaceID = placeviewmodel.SpecificPlace.Place_Id,
                PlaceName = placeviewmodel.SpecificPlace.Place_Name,
                dbimage = placeviewmodel.SpecificPlace.dbimage,
                Description = placeviewmodel.SpecificPlace.Description,
                TotalAmount = placeviewmodel.SpecificPlace.Place_Price,
                PlaceType = placeviewmodel.SpecificPlace.Place_Type,
            };

            return View("Create", bookingViewModel);
        }

        return View(paymentViewModel);
    }


    [HttpPost]
    public IActionResult PaymentConfirmed(int TotalAmountAfterDiss)
    {
        return View("Success", TotalAmountAfterDiss);
    }

    
}
