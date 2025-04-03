using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using ToDo.Data;
using ToDo.Domain.Models;

namespace ToDo.Migrations.Workers
{
    public class MigrationWorker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
    {
        public const string ActivitySourceName = "Migrations";
        private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await RunMigrationAsync(dbContext, cancellationToken);
                await SeedDataAsync(dbContext, cancellationToken);
            }
            catch (Exception ex)
            {
                activity?.RecordException(ex);
                throw;
            }

            hostApplicationLifetime.StopApplication();
        }

        private static async Task RunMigrationAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // Run migration in a transaction to avoid partial migration if it fails.
                await dbContext.Database.MigrateAsync(cancellationToken);
            });
        }

        private static async Task SeedDataAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            User user = new()
            {
                Id = new Guid("b40bf22a-12c0-4358-b62a-4072f08a0579"),
                Username = "testuser"
            };

            TodoItem todoItem = new()
            {
                Completed = true,
                Title = "Start workshop",
                Id = Guid.NewGuid(),
                UserId = new Guid("b40bf22a-12c0-4358-b62a-4072f08a0579"),
            };

            UserStat userStat = new()
            {
                UserId = new Guid("b40bf22a-12c0-4358-b62a-4072f08a0579"),
                Points = 0,
                StreakDays = 0,
                Level = 1
            };

            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Users", cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM ToDos", cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM UserStats", cancellationToken);

                // Seed the database
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                await dbContext.Users.AddAsync(user, cancellationToken);
                await dbContext.ToDos.AddAsync(todoItem, cancellationToken);
                await dbContext.UserStats.AddAsync(userStat, cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            });
        }
    }
}
