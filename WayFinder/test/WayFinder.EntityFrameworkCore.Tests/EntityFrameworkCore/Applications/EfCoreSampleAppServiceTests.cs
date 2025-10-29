using WayFinder.Samples;
using Xunit;

namespace WayFinder.EntityFrameworkCore.Applications;

[Collection(WayFinderTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<WayFinderEntityFrameworkCoreTestModule>
{

}
