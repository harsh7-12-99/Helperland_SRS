using Helperland.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
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
        [HttpPost]
        public JsonResult CheckPincode(string zipcode)
        {

            if (zipcode == null || zipcode.Length > 7 || !Regex.IsMatch(zipcode, @"^[0-9]{6}$"))
            {
                return Json(false);
            }
            else
            {
                if (IsServiceAvailable(zipcode))
                {
                    return Json(true);
                }
                else
                    return Json("We are not providing service in this area. We’ll notify you if any helper would start working near your area.");
            }
        }
        [HttpPost]
        public bool IsServiceAvailable(string strZipcode)
        {
            if (strZipcode == null)
            {
                return false;
            }
            else
            {
                User verify = _context.Users.FirstOrDefault(x => x.ZipCode.Equals(strZipcode));
                try
                {
                    if (verify != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    return false;
                }

            }
        }
        [HttpPost]
        public string ServiceSchedule([FromBody] ServiceRequest serviceRequest)
        {

            serviceRequest.UserId = 1;
            serviceRequest.ServiceId = 1;
            serviceRequest.CreatedDate = DateTime.Now;
            serviceRequest.ServiceHourlyRate = 18;
            _context.ServiceRequests.Add(serviceRequest);
            _context.SaveChanges();
            return "true";
        }
        [HttpPost]
        public JsonResult Details([FromBody] ServiceRequestAddress address)
        {
            _context.ServiceRequestAddresses.Add(address);
            _context.SaveChanges();
            return Json(true);
        }
    }
}

