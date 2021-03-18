using DynPlugins.Metadata;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Extensions
{
	internal static class InputParametersExtension
	{
		internal static Entity GetTargetEntity(this ParameterCollection inputParameters)
		{
			return inputParameters.GetEntity(InputParameterNames.Target);
		}

		private static Entity GetEntity(this ParameterCollection inputParameters, string parameterName)
		{
			object value;
			if (inputParameters.TryGetValue(parameterName, out value))
			{
				Entity result = value as Entity;
				if (result == null)
				{
					throw new InvalidPluginExecutionException($"The value type of InputParameter with name = {parameterName} is not \"entity\"");
				}
				return result;
			}
			else
			{
				throw new InvalidPluginExecutionException($"InputParameters does not contain parameter with name = {parameterName}");
			}
		}
	}
}
