using ClaimMonthlySystem.Models;
using ClaimMonthlySystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;
using System.Net;
using System.Security.Cryptography.Xml;

namespace ClaimMonthlySystem.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IClaimService _claimService;

        public ManagerController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Home");

            var pendingClaims = _claimService.GetPendingClaims();
            return View(pendingClaims);
        }

        [HttpPost]
        public IActionResult ApproveClaim(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Home");

            _claimService.UpdateClaimStatus(id, ClaimStatus.Approved);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RejectClaim(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Home");

            _claimService.UpdateClaimStatus(id, ClaimStatus.Rejected);
            return RedirectToAction("Index");
        }

        public IActionResult ViewDocument(int claimId, int documentId)
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Home");

            var claim = _claimService.GetClaimById(claimId);
            var document = claim?.Documents.FirstOrDefault(d => d.Id == documentId);

            if (document == null)
                return NotFound();

            return File(document.FileContent, document.ContentType, document.FileName);
        }
    }
}

//References
//Anon., 2009. Allow multiple roles to access controller action. [Online] 
//Available at: https://stackoverflow.com/questions/700166/allow-multiple-roles-to-access-controller-action
//Anon., 2024. Role-based authorization in ASP.NET Core. [Online] 
//Available at: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-9.0
//Anon., 2025. Role Based Access Of An MVC Application. [Online] 
//Available at: https://www.c-sharpcorner.com/UploadFile/rahul4_saxena/role-based-access-of-an-mvc-application/

