using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using WayFinder.DestinosTuristicosDTOs;
using WayFinder.EntityFrameworkCore;
using Xunit;

namespace WayFinder.DestinosTuristicos
{
    [Collection(WayFinderTestConsts.CollectionDefinitionName)]
    public class EfCoreDestinoTuristicoAppService_Tests : DestinoTurisiticoAppService_Tests<WayFinderEntityFrameworkCoreTestModule>
    {
    }
}