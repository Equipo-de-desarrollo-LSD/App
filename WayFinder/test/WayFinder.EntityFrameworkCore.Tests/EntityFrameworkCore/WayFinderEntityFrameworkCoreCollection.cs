using Xunit;

namespace WayFinder.EntityFrameworkCore;

[CollectionDefinition(WayFinderTestConsts.CollectionDefinitionName)]
public class WayFinderEntityFrameworkCoreCollection : ICollectionFixture<WayFinderEntityFrameworkCoreFixture>
{

}
