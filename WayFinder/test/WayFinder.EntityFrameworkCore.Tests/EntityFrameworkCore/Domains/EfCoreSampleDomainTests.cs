using WayFinder.Samples;
using Xunit;

namespace WayFinder.EntityFrameworkCore.Domains;

[Collection(WayFinderTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<WayFinderEntityFrameworkCoreTestModule>
{

}
