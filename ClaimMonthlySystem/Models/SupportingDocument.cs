namespace ClaimMonthlySystem.Models
{
    //the documents information that the user submits
    public class SupportingDocument
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
//References
//Anon., 2010. Models In ASP.NET MVC 5. [Online] 
//Available at: https://www.c-sharpcorner.com/article/models-in-asp-net-mvc5/
//Anon., 2010. MVC :: What is a model?. [Online] 
//Available at: https://stackoverflow.com/questions/4221632/mvc-what-is-a-model
//Anon., 2022. Understanding Models, Views, and Controllers (C#). [Online] 
//Available at: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/overview/understanding-models-views-and-controllers-cs
