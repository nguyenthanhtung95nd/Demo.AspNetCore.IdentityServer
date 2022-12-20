using Microsoft.AspNetCore.Identity;

namespace Demo.IDP.Entities
{
    /// <summary>
    /// class must inherit from the IdentityUser class to accept all the default fields that Identity provides
    /// additionally, we extend the IdentityUser class with our properties
    /// </summary>
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }
}