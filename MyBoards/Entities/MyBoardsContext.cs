using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using MyBoards.Entities.View;

namespace MyBoards.Entities
{
    public class MyBoardsContext : DbContext
    {
        public MyBoardsContext(DbContextOptions<MyBoardsContext> options) : base(options) 
        { 

        }

        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Epic> Epics { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<WorkItemState> WorkItemsState { get; set; }
        public DbSet<WorkItemTag> WorkItemTag { get; set; }
        public DbSet<TopAuthor> ViewTopAuthors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(eb =>
            {
                eb.Property(x => x.CreatedDate).HasDefaultValueSql("getutcdate()");
                eb.Property(x => x.UpdatedDate).ValueGeneratedOnUpdate();
                eb.HasOne(x => x.Author)
                  .WithMany(x => x.Comments)
                  .HasForeignKey(x => x.AuthorId)
                  .OnDelete(DeleteBehavior.ClientCascade);
            });

            modelBuilder.Entity<Epic>(eb =>
            {
                eb.Property(x => x.EndDate).HasPrecision(3);
            });

            modelBuilder.Entity<Issue>(eb =>
            {
                eb.Property(x => x.Efford).HasColumnType("decimal(5,2)");
            });

            modelBuilder.Entity<Task>(eb =>
            {
                eb.Property(x => x.RemaningWork).HasPrecision(14, 2);
                eb.Property(x => x.Activity).HasMaxLength(200);
            });

            modelBuilder.Entity<WorkItem>(eb =>
            {
                eb.Property(x => x.Area).HasColumnType("varchar(200)");
                eb.Property(x => x.IterationPath).HasColumnName("Iteration_Path");
                eb.Property(x=>x.Priority).HasDefaultValue(1);

                //Relacja jeden WorkItem do wielu komentarzy
                eb.HasMany(c => c.Comments)
                  .WithOne(wi => wi.WorkItem)
                  .HasForeignKey(fk => fk.WorkItemId);

                // Relacja wiele WorkItem do wielu Tag
                eb.HasMany(w => w.Tags)
                  .WithMany(t => t.WorkItems)
                  .UsingEntity<WorkItemTag>(

                    w => w.HasOne(wit => wit.Tag)
                    .WithMany()
                    .HasForeignKey(wit => wit.TagId),

                    w => w.HasOne(wit => wit.WorkItem)
                    .WithMany()
                    .HasForeignKey(wit => wit.WorkItemId),

                    w =>
                    {
                        w.HasKey(wit => new {wit.WorkItemId, wit.TagId});
                        w.Property(x => x.PublicationDate).HasDefaultValueSql("getutcdate()");
                    }
                  );

                // Relacja wiele workitem do jeden state
                eb.HasOne(wi => wi.State)
                  .WithMany()
                  .HasForeignKey(wi => wi.StateId);
            });

            modelBuilder.Entity<User>(eb =>
            {
                eb.HasOne(a => a.Address)
                  .WithOne(u => u.User)
                  .HasForeignKey<Address>(x => x.UserId);

                eb.HasMany(wi => wi.WorkItems)
                  .WithOne(u => u.Author)
                  .HasForeignKey(fk => fk.AuthorId);
            });

            modelBuilder.Entity<WorkItemState>(eb =>
            {
                eb.HasData(new WorkItemState() { Id = 1, Value = "To do" }, 
                           new WorkItemState() { Id = 2, Value = "Doing" }, 
                           new WorkItemState() { Id = 3, Value = "Done" });
            });
            modelBuilder.Entity<TopAuthor>(eb =>
            {
                eb.ToView("View_TopAuthors")
                .HasNoKey();
            });
        }
    } 
}   
