using DynPlugins.Extensions;
using DynPlugins.Validations;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Plugins
{
	internal class PluginExecutionFacade
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly Lazy<IPluginExecutionContext> _lazyPluginExecutionContext;
		private readonly Lazy<IOrganizationService> _lazyOrganizationService;
		private readonly Lazy<ITracingService> _lazyTracingService;

		public PluginExecutionFacade(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider.ArgumentNotNull(nameof(serviceProvider));
			_lazyPluginExecutionContext = new Lazy<IPluginExecutionContext>(GetPluginExecutionContext);
			_lazyOrganizationService = new Lazy<IOrganizationService>(GetOrganizationService);
			_lazyTracingService = new Lazy<ITracingService>(GetTracingService);
		}

		public IPluginExecutionContext Context => _lazyPluginExecutionContext.Value;

		public IOrganizationService OrganizationService => _lazyOrganizationService.Value;

		public ITracingService TracingService => _lazyTracingService.Value;

		private IPluginExecutionContext GetPluginExecutionContext()
		{
			return _serviceProvider.GetContext();
		}

		private IOrganizationService GetOrganizationService()
		{
			return _serviceProvider.GetOrganizationService(Context);
		}

		private ITracingService GetTracingService()
		{
			return _serviceProvider.GetTracingService();
		}
	}
}
