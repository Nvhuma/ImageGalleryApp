using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        :base(dbContextOptions)
        {
            
        }

        public DbSet<Tag> tag { get; set;}
        public DbSet<Comment> comments {get; set;}
        public DbSet<Image> images {get; set;}
         public DbSet<ImageTag> ImageTags {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define composite primary key for ImageTag
            modelBuilder.Entity<ImageTag>()
                .HasKey(it => new { it.ImageId, it.TagId });

            // Configure relationships
            modelBuilder.Entity<ImageTag>()
                .HasOne(it => it.Image)
                .WithMany(i => i.ImageTags)
                .HasForeignKey(it => it.ImageId);

            modelBuilder.Entity<ImageTag>()
                .HasOne(it => it.Tag)
                .WithMany(t => t.ImageTags)
                .HasForeignKey(it => it.TagId);


            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Image)
                .WithMany(i => i.Comments)
                .HasForeignKey(c => c.ImageId);

           
        }
    }
}
    
