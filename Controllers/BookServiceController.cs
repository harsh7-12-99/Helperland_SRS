using Helperland.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Helperland.Data;
using Microsoft.AspNetCore.Http;

namespace Helperland.Controllers
{
    public class BookServiceController : Controller
    {
        private readonly HelperlandContext _context;

        public BookServiceController(HelperlandContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult BookService()
        {
            return View();
        }
        public string IsServiceAvailable(string entered_zipcode)
        {

            var zipcode = _context.Users.Where(x => x.ZipCode == entered_zipcode).SingleOrDefault();
            string serviceAvailable;
            if (zipcode == null)
            {
                serviceAvailable = "false";
            }
            else
            {
                serviceAvailable = "true";
            }
            return serviceAvailable;
        }

        

        [HttpPost]
        public IActionResult ServiceSchedule(ServiceRequest obj)
        {
            ServiceRequest servicerequest = new ServiceRequest();
            servicerequest.Comments = obj.Comments;
            servicerequest.HasPets = obj.HasPets;

            _context.ServiceRequests.Add(obj);
            _context.SaveChanges();
            return View() ;
        }
        [HttpPost]
        public IActionResult Details(ServiceRequestAddress obj1)
        {
            
            ServiceRequestAddress serviceRequestAddress = new ServiceRequestAddress();
            serviceRequestAddress.ServiceRequestId = obj1.ServiceRequestId;
            serviceRequestAddress.AddressLine1 = obj1.AddressLine1;
            serviceRequestAddress.AddressLine2 = obj1.AddressLine2;
            serviceRequestAddress.PostalCode = obj1.PostalCode;
            serviceRequestAddress.City = obj1.City;
            _context.ServiceRequestAddresses.Add(obj1);
            _context.SaveChanges();
            return View();
        }
        public IActionResult CheckForLogin()
        {
            var check = HttpContext.Session.GetInt32("UserID_Session");
            if (check != null)
            {
                return View();
            }
            string x = "You must be logged in to book a service";
            TempData["LoginNecessary"] = x;
            return RedirectToAction("Index", "Home");
        }
    }
}
