﻿using Cts.AppServices.ActionTypes;
using Cts.AppServices.AutoMapper;
using Cts.AppServices.Offices;
using Cts.Domain.ActionTypes;
using Cts.Domain.Offices;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Cts.AppServices.ServiceCollectionExtensions;

public static class AppServices
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

        // Action Types
        services.AddScoped<IActionTypeManager, ActionTypeManager>();
        services.AddScoped<IActionTypeAppService, ActionTypeAppService>();

        // Offices
        services.AddScoped<IOfficeManager, OfficeManager>();
        services.AddScoped<IOfficeAppService, OfficeAppService>();

        // Add all validators
        services.AddValidatorsFromAssemblyContaining(typeof(AppServices));
    }
}
