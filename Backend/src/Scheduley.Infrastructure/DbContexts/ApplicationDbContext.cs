using System;
using Microsoft.EntityFrameworkCore;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.Domain.Enums;

namespace Scheduley.Infrastructure.DbContexts;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Message> Messages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<WhatsAppMessage> WhatsAppMessages { get; set; }
    public DbSet<AppMessage> AppMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder
            .Entity<Message>()
            .HasDiscriminator<MessageType>("MessageType")
            .HasValue<WhatsAppMessage>(MessageType.WhatsApp)
            .HasValue<AppMessage>(MessageType.App);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder
            .Entity<User>()
            .HasData(
                [
                    new User()
                    {
                        Email = "ahmad.mhfz1412@gmail.com",
                        Name = "Ahmad Mahfouz",
                        UserID = new Guid("7e91312b-5175-47cf-ab07-8f293772feed"),
                        BucketName = new Guid("0a04b8fb-7416-4bfc-8ef0-7a505fe803a1"),
                        CreatedAt = new DateTime(
                            2025,
                            4,
                            14,
                            3,
                            58,
                            8,
                            478,
                            DateTimeKind.Utc
                        ).AddTicks(2445),
                        LastLogin = new DateTime(
                            2025,
                            4,
                            14,
                            3,
                            58,
                            8,
                            478,
                            DateTimeKind.Utc
                        ).AddTicks(2448),
                        UserRole = 0,
                    },
                    new User()
                    {
                        Email = "thecityhunterhd@gmail.com",
                        Name = "Molly",
                        UserID = new Guid("4bf68c7d-f166-4dc0-92cd-9ae4c519da61"),
                        BucketName = new Guid("bb5c4df4-1b56-4f30-9d45-ec7a57743ce6"),
                        CreatedAt = new DateTime(
                            2025,
                            4,
                            14,
                            3,
                            58,
                            8,
                            478,
                            DateTimeKind.Utc
                        ).AddTicks(2476),
                        LastLogin = new DateTime(
                            2025,
                            4,
                            14,
                            3,
                            58,
                            8,
                            478,
                            DateTimeKind.Utc
                        ).AddTicks(2476),
                        UserRole = 0,
                    },
                ]
            );
    }
}
