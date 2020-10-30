using AutoFixture.Kernel;
using System;
using System.Reflection;

namespace Demo.Management.StarterKit.Logging.UnitTests
{
    public class UriSpecimenBuilder : ISpecimenBuilder
	{
		public object Create(object request, ISpecimenContext context)
		{
			if (request is PropertyInfo propertyInfo)
			{
				if (propertyInfo.PropertyType == typeof(Uri))
				{
					var guid = Guid.NewGuid();
					return new Uri($"https://{guid}");
				}
			}

			return new NoSpecimen();
		}
	}
}