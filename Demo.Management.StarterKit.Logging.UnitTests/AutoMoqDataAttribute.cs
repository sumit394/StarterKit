using AutoFixture.Xunit2;

namespace Demo.Management.StarterKit.Logging.UnitTests
{
	public class AutoMoqDataAttribute : AutoDataAttribute
	{
		public AutoMoqDataAttribute()
			: base(FixtureHelper.GetDefaultFixture)
		{
		}
	}
}