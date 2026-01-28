using System;
using System.Reflection;
using System.Security.Cryptography.Xml;
namespace ClaimMonthlySystem.Models
{
    //information to be stroed for claims that are submitted
    public class Claim
    {
        public int Id { get; set; }
        public int LecturerId { get; set; }
        public string LecturerName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount => HoursWorked * HourlyRate;
        public string AdditionalNotes { get; set; }
        public ClaimStatus Status { get; set; }
        public List<SupportingDocument> Documents { get; set; } = new List<SupportingDocument>();
    }
}
//References
//Anon., 2010. Models In ASP.NET MVC 5. [Online] 
//Available at: https://www.c-sharpcorner.com/article/models-in-asp-net-mvc5/
//Anon., 2010. MVC :: What is a model?. [Online] 
//Available at: https://stackoverflow.com/questions/4221632/mvc-what-is-a-model
//Anon., 2022. Understanding Models, Views, and Controllers (C#). [Online] 
//Available at: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/overview/understanding-models-views-and-controllers-cs

