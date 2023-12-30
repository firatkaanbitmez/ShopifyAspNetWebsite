// Data/DataContext.cs
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopAppProject.Models; // Assuming User class is in the Models namespace


namespace ShopAppProject.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Identity tablolarını ekleyin
            _ = builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            _ = builder.Entity<IdentityRole>().ToTable("AspNetRoles");
            _ = builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
            _ = builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
            _ = builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
            _ = builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
            _ = builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");

            _ = builder.Entity<Order>()
    .HasOne(o => o.User)  // Update this to reference the User property in the Order entity
    .WithMany()
    .HasForeignKey(o => o.UserId);

        }



        public DbSet<Product> Products { get; set; }
        public DbSet<Product> Productler => Set<Product>();
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CommentReaction> CommentReactions { get; set; }
        public DbSet<UserProductList> UserProductLists { get; set; }
        public DbSet<Deals> Deals { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Sold> Solds { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<Comment> Comments { get; set; }

    }
}
