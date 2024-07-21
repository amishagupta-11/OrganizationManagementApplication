using Microsoft.EntityFrameworkCore;
using OrganizationManagementApplication.Models;

namespace OrganizationManagementApplication.Data
{
    public class OrganizationManagementContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationManagementContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the context.</param>
        public OrganizationManagementContext(DbContextOptions<OrganizationManagementContext> options)
            : base(options)
        {
        }
        /// <summary>
        /// Gets or sets the DbSet of employees, representing the collection of Employee entities in the database.
        /// </summary>
        public DbSet<Employee> Employees { get; set; }
    }
    
}
