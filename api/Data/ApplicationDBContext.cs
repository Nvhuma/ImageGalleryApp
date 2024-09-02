using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }

        public DbSet<Tag> Tag { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<ImageTag> ImageTags { get; set; }

        public DbSet<UserPasswordHistory> UserPasswordHistory { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            List<IdentityRole> roles = new List<IdentityRole>
           {
             new IdentityRole
             {
                Name = "Admin",
                NormalizedName = "Admin"
             },
              new IdentityRole
             {
                Name = "User",
                NormalizedName = "User"
             },
           };
            //seed roles 
            modelBuilder.Entity<IdentityRole>().HasData(roles);

            // Fluent API configurations
            modelBuilder.Entity<AppUser>()
                   .Property(u => u.Id)
                   .HasColumnName("UserID");



            modelBuilder.Entity<UserPasswordHistory>()
           .HasOne(uph => uph.AppUser)
           .WithMany(u => u.UserPasswordHistories)
           .HasForeignKey(uph => uph.AppUserID);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.AppUser)
                .WithMany(u => u.Images)
                .HasForeignKey(i => i.UserId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.AppUser)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Image)
                .WithMany(i => i.Comments)
                .HasForeignKey(c => c.ImageId);

            modelBuilder.Entity<ImageTag>()
                    .HasKey(it => it.ImageTagID);

            modelBuilder.Entity<ImageTag>()
                .Property(it => it.ImageTagID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ImageTag>()
                .HasOne(it => it.Image)
                .WithMany(i => i.ImageTags)
                .HasForeignKey(it => it.ImageId);

            modelBuilder.Entity<ImageTag>()
               .HasOne(it => it.Tag)
               .WithMany(t => t.ImageTags)
               .HasForeignKey(it => it.TagId);
        }
    }
}

