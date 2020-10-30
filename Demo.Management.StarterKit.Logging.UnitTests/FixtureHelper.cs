using AutoFixture;
using AutoFixture.AutoMoq;

namespace Demo.Management.StarterKit.Logging.UnitTests
{
	public static class FixtureHelper
	{
		public static IFixture GetDefaultFixture()
		{
			var fixture = new Fixture()
				.Customize(new AutoMoqCustomization());

			fixture.Customizations.Add(new UriSpecimenBuilder());
			return fixture;
		}
	}
}