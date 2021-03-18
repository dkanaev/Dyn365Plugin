using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Validations
{
	internal static class Guard
	{
		internal static T ArgumentNotNull<T>(this T parameter, string paramName) where T : class
		{
			if (ReferenceEquals(parameter, null))
			{
				throw new InvalidPluginExecutionException($"Argument {paramName} is null");
			}
			return parameter;
		}
	}
}
