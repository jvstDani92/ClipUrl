﻿using ClipUrl.Domain.Interfaces;
using ClipUrl.Infrastructure.Data;
using ClipUrl.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClipUrl.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing DB Connection String.");

            services.AddDbContextPool<ClipUrlDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsAssembly(typeof(ClipUrlDbContext).Assembly.GetName().Name);
                    sql.EnableRetryOnFailure(5);
                });
            });

            services.AddScoped(typeof(IRepository<>), typeof(ClipUrlDbRepository<>));

            return services;
        }
    }
}
