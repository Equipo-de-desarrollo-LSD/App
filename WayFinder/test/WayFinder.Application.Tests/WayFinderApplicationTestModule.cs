using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using WayFinder.DestinosTuristicosDTOs;
using Volo.Abp;
using Volo.Abp.Modularity;
using WayFinder.EntityFrameworkCore; 


namespace WayFinder;

[DependsOn(
    typeof(WayFinderApplicationModule),
    typeof(WayFinderDomainTestModule),
    typeof(WayFinderEntityFrameworkCoreTestModule)
)]
public class WayFinderApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Siempre usar un mock para IBuscarCiudadService en los tests
        var citySearchServiceMock = Substitute.For<IBuscarCiudadService>();
        context.Services.AddSingleton(citySearchServiceMock);
    }

}
/*
 * ﻿using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using TravelBuddy.CitySearch;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace TravelBuddy;

[DependsOn(
    typeof(TravelBuddyApplicationModule),
    typeof(TravelBuddyDomainTestModule)
)]
public class TravelBuddyApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Siempre usar un mock para ICitySearchService en los tests
        var citySearchServiceMock = Substitute.For<ICitySearchService>();
        context.Services.AddSingleton(citySearchServiceMock);
    }
}
