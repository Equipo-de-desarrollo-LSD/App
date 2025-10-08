using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayFinder.EntityFrameworkCore;
using Xunit;

namespace WayFinder.DestinosTuristicos
{
    [Collection(WayFinderTestConsts.CollectionDefinitionName)]
    public class EfCoreDestinoTuristicoAppService_Tests : DestinoTurisiticoAppService_Tests<WayFinderEntityFrameworkCoreTestModule>
    {
    }
}
