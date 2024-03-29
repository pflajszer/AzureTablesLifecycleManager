﻿using Azure.Data.Tables;
using AzureTablesLifecycleManager.AzureDAL.APIGateway;
using AzureTablesLifecycleManager.Lib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AzureTablesLifecycleManager.Lib.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAzureTablesLifecycleManagement(this IServiceCollection services)
        {
            services.AddSingleton(p => new TableServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));
            services.AddSingleton<ITableRepository, TableRepository>();
            services.AddSingleton<ITableManager, TableManager>();
            services.AddSingleton<IQueryBuilder, QueryBuilder>();
        }

        public static void AddAzureTablesLifecycleManagement(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(p => new TableServiceClient(connectionString));
            services.AddSingleton<ITableRepository, TableRepository>();
            services.AddSingleton<ITableManager, TableManager>();
            services.AddSingleton<IQueryBuilder, QueryBuilder>();
        }
    }
}
