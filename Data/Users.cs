using Microsoft.AspNetCore.Identity;
using tournament_blazor.Models.db_manager;

namespace tournament_blazor.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class Users : IdentityUser
    {
        public ICollection<Tournaments> Tournaments { get; set; } = new HashSet<Tournaments>();
    }
}
