using Microsoft.EntityFrameworkCore;
using WorkflowMvp.Api.Models;

namespace WorkflowMvp.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<RequestEntity> Requests => Set<RequestEntity>();
    public DbSet<RequestCommentEntity> RequestComments => Set<RequestCommentEntity>();
    public DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RequestEntity>(entity =>
        {
            entity.ToTable("Requests");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(x => x.CamundaProcessInstanceKey).HasMaxLength(64);
            entity.HasMany(x => x.Comments)
                .WithOne(x => x.Request)
                .HasForeignKey(x => x.RequestId);
        });

        modelBuilder.Entity<RequestCommentEntity>(entity =>
        {
            entity.ToTable("RequestComments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Author).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Comment).HasMaxLength(2000).IsRequired();
        });

        modelBuilder.Entity<AuditLogEntity>(entity =>
        {
            entity.ToTable("AuditLog");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Action).HasMaxLength(80).IsRequired();
            entity.Property(x => x.PerformedBy).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Details).HasMaxLength(2000).IsRequired();
        });
    }
}
