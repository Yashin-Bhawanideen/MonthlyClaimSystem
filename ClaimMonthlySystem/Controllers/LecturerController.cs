using ClaimMonthlySystem.Models;
using ClaimMonthlySystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;
using System.Net;
using System.Security.Cryptography.Xml;

namespace ClaimMonthlySystem.Controllers
{
    public class LecturerController : Controller
    {
        private readonly IClaimService _claimService;

        public LecturerController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Login", "Home");

            var lecturerId = int.Parse(HttpContext.Session.GetString("UserId"));
            var lecturerName = HttpContext.Session.GetString("Name");
            
            // Debug information
            System.Diagnostics.Debug.WriteLine($"Lecturer Index - LecturerId: {lecturerId}, LecturerName: {lecturerName}");
            System.Diagnostics.Debug.WriteLine($"Debug Info: {_claimService.GetDebugInfo()}");
            
            var claims = _claimService.GetClaimsByLecturer(lecturerId);
            System.Diagnostics.Debug.WriteLine($"Found {claims.Count} claims for lecturer {lecturerId}");
            
            return View(claims);
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Login", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult SubmitClaim(Claim claim, IFormFile supportingDocument)
        {
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                try
                {
                    var lecturerId = int.Parse(HttpContext.Session.GetString("UserId"));
                    var lecturerName = HttpContext.Session.GetString("Name");
                    
                    claim.LecturerId = lecturerId;
                    claim.LecturerName = lecturerName;

                    System.Diagnostics.Debug.WriteLine($"Submitting claim - LecturerId: {lecturerId}, LecturerName: {lecturerName}");
                    System.Diagnostics.Debug.WriteLine($"Before adding - Debug Info: {_claimService.GetDebugInfo()}");

                    _claimService.AddClaim(claim);

                    System.Diagnostics.Debug.WriteLine($"After adding - Debug Info: {_claimService.GetDebugInfo()}");

                    if (supportingDocument != null && supportingDocument.Length > 0)
                    {
                        // File validation
                        var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                        var fileExtension = Path.GetExtension(supportingDocument.FileName).ToLower();
                        
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("", "Only PDF, DOCX, and XLSX files are allowed.");
                            return View(claim);
                        }

                        if (supportingDocument.Length > 5 * 1024 * 1024) // 5MB limit
                        {
                            ModelState.AddModelError("", "File size must be less than 5MB.");
                            return View(claim);
                        }

                        using var memoryStream = new MemoryStream();
                        supportingDocument.CopyTo(memoryStream);

                        var document = new SupportingDocument
                        {
                            FileName = supportingDocument.FileName,
                            FileContent = memoryStream.ToArray(),
                            ContentType = supportingDocument.ContentType,
                            ClaimId = claim.Id
                        };

                        _claimService.AddDocumentToClaim(claim.Id, document);
                    }

                    TempData["Success"] = "Claim submitted successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error submitting claim: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while submitting your claim. Please try again.");
                    return View(claim);
                }
            }

            
            return View(claim);
        }

        public IActionResult ClaimDetails(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Login", "Home");

            var claim = _claimService.GetClaimById(id);
            if (claim == null || claim.LecturerId != int.Parse(HttpContext.Session.GetString("UserId")))
            {
                return NotFound();
            }

            return View(claim);
        }

        public IActionResult ViewDocument(int claimId, int documentId)
        {
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Login", "Home");

            var claim = _claimService.GetClaimById(claimId);
            var document = claim?.Documents.FirstOrDefault(d => d.Id == documentId);

            if (document == null || claim.LecturerId != int.Parse(HttpContext.Session.GetString("UserId")))
            {
                return NotFound();
            }

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

