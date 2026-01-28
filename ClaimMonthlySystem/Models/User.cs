namespace ClaimMonthlySystem.Models
{
    //information about the user 
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Lecturer" or "Manager"
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
//References
//Anon., 2010. Models In ASP.NET MVC 5. [Online] 
//Available at: https://www.c-sharpcorner.com/article/models-in-asp-net-mvc5/
//Anon., 2010. MVC :: What is a model?. [Online] 
//Available at: https://stackoverflow.com/questions/4221632/mvc-what-is-a-model
//Anon., 2022. Understanding Models, Views, and Controllers (C#). [Online] 
//Available at: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/overview/understanding-models-views-and-controllers-cs
