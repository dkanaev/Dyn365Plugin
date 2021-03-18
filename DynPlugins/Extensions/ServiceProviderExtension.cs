using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Extensions
{
	internal static class ServiceProviderExtension
	{
		internal static T GetService<T>(this IServiceProvider serviceProvider)
		{
			var type = typeof(T);
			var result = (T)serviceProvider.GetService(type);
			if (result == null)
			{
				throw new InvalidPluginExecutionException($"The instance of {type} is not provided");
			}
			return result;
		}

		internal static IPluginExecutionContext GetContext(this IServiceProvider serviceProvider)
		{
			return serviceProvider.GetService<IPluginExecutionContext>();
		}

		internal static ITracingService GetTracingService(this IServiceProvider serviceProvider)
		{
			return serviceProvider.GetService<ITracingService>();
		}

		internal static IOrganizationService GetOrganizationService(this IServiceProvider serviceProvider, IPluginExecutionContext context)
		{
			var serviceFactory = serviceProvider.GetService<IOrganizationServiceFactory>();
			var result = serviceFactory.CreateOrganizationService(context.UserId);
			return result;
		}
	}
}
