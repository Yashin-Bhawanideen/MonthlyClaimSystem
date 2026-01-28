using ClaimMonthlySystem.Models;
using System.Security.Cryptography.Xml;

namespace ClaimMonthlySystem.Services
{
    //using interface
    public class ClaimService : IClaimService
    {
        private static readonly List<Claim> _claims = new();
        private static readonly List<User> _users = new();
        private static int _claimIdCounter = 1;
        private static int _documentIdCounter = 1;
        private static int _userIdCounter = 3;
        private static bool _isInitialized = false;
        private static readonly object _lock = new object();

        public ClaimService()
        {
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            lock (_lock)
            {
                if (_isInitialized) return;

                // Only ONE manager - Jack
                //Jack only has access
                _users.AddRange(new[]
                {
                    new User { 
                        Id = 1, 
                        Username = "Jack", 
                        Password = "J1", 
                        Role = "Manager", 
                        Name = "Jack Wilson",
                        Email = "jack.wilson@university.co.za",
                        Department = "Academic Affairs",
                        CreatedDate = DateTime.Now.AddDays(-30)
                    },
                    new User { 
                        Id = 2, 
                        Username = "lecturer1", 
                        Password = "pass123", 
                        Role = "Lecturer", 
                        Name = "Dr. John Smith",
                        Email = "john.smith@university.ac.za",
                        Department = "Computer Science",
                        CreatedDate = DateTime.Now.AddDays(-25)
                    }
                });

                _isInitialized = true;
                
                //Log initialization
                System.Diagnostics.Debug.WriteLine("ClaimService initialized with users: " + _users.Count);
            }
        }

        public List<Claim> GetClaims() 
        {
            lock (_lock)
            {
                System.Diagnostics.Debug.WriteLine($"GetClaims called - returning {_claims.Count} claims");
                return new List<Claim>(_claims);
            }
        }
        
        public List<Claim> GetClaimsByLecturer(int lecturerId) 
        {
            lock (_lock)
            {
                var claims = _claims.Where(c => c.LecturerId == lecturerId).ToList();
                System.Diagnostics.Debug.WriteLine($"GetClaimsByLecturer({lecturerId}) - returning {claims.Count} claims");
                return claims;
            }
        }
        
        public List<Claim> GetPendingClaims() 
        {
            lock (_lock)
            {
                return _claims.Where(c => c.Status == ClaimStatus.Pending).ToList();
            }
        }
        
        public Claim GetClaimById(int id) 
        {
            lock (_lock)
            {
                return _claims.FirstOrDefault(c => c.Id == id);
            }
        }
        
        public void AddClaim(Claim claim)
        {
            lock (_lock)
            {
                claim.Id = _claimIdCounter++;
                claim.SubmissionDate = DateTime.Now;
                claim.Status = ClaimStatus.Pending;
                _claims.Add(claim);
                
                System.Diagnostics.Debug.WriteLine($"Claim added - ID: {claim.Id}, Lecturer: {claim.LecturerName}, Total claims: {_claims.Count}");
                
                // Debug: Print all claims
                foreach (var c in _claims)
                {
                    System.Diagnostics.Debug.WriteLine($"Claim {c.Id}: LecturerId={c.LecturerId}, LecturerName={c.LecturerName}, Hours={c.HoursWorked}");
                }
            }
        }
        
        public void UpdateClaimStatus(int claimId, ClaimStatus status)
        {
            lock (_lock)
            {
                var claim = GetClaimById(claimId);
                if (claim != null)
                {
                    claim.Status = status;
                }
            }
        }
        
        public void AddDocumentToClaim(int claimId, SupportingDocument document)
        {
            lock (_lock)
            {
                var claim = GetClaimById(claimId);
                if (claim != null)
                {
                    document.Id = _documentIdCounter++;
                    document.UploadDate = DateTime.Now;
                    claim.Documents.Add(document);
                }
            }
        }
        
        public List<User> GetUsers() 
        {
            lock (_lock)
            {
                return new List<User>(_users);
            }
        }
        
        public User Authenticate(string username, string password) 
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
            }
        }

        public bool Register(User user)
        {
            lock (_lock)
            {
                if (UsernameExists(user.Username))
                    return false;

                // Force role to be Lecturer only - no manager registration allowed
                user.Role = "Lecturer";
                user.Id = _userIdCounter++;
                user.CreatedDate = DateTime.Now;
                _users.Add(user);
                return true;
            }
        }

        public bool UsernameExists(string username)
        {
            lock (_lock)
            {
                return _users.Any(u => u.Username.ToLower() == username.ToLower());
            }
        }

        // Debug method to check current state
        public string GetDebugInfo()
        {
            lock (_lock)
            {
                return $"Users: {_users.Count}, Claims: {_claims.Count}, NextClaimId: {_claimIdCounter}";
            }
        }
    }
}

//References
//Bala, 2014. Understanding Interfaces in C#. [Online] 
//Available at: https://www.c-sharpcorner.com/UploadFile/sekarbalag/Interface-In-CSharp/
//MNsr, 2012. C# How to use interfaces. [Online] 
//Available at: https://stackoverflow.com/questions/7762291/c-sharp-how-to-use-interfaces
//W3schools, 2025. C# Interface. [Online] 
//Available at: https://www.w3schools.com/cs/cs_interface.php
//Anon., 2009. Allow multiple roles to access controller action. [Online] 
//Available at: https://stackoverflow.com/questions/700166/allow-multiple-roles-to-access-controller-action
//Anon., 2024. Role-based authorization in ASP.NET Core. [Online] 
//Available at: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-9.0
//Anon., 2025. Role Based Access Of An MVC Application. [Online] 
//Available at: https://www.c-sharpcorner.com/UploadFile/rahul4_saxena/role-based-access-of-an-mvc-application/

