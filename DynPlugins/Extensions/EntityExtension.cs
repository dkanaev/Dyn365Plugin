using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Extensions
{
	internal static class EntityExtension
	{
		internal static Entity Clone(this Entity entity)
		{
			Entity result = new Entity(entity.LogicalName);
			foreach (var attr in entity.Attributes)
			{
				if (attr.Key.ToLower() == entity.LogicalName.ToLower() + "id" || attr.Key.ToLower() == "activityid")
				{
					continue;
				}
				result[attr.Key] = attr.Value;
			}
			return result;
		}

		internal static T GetAttributeValueWithDebug<T>(this Entity entity, string attributeLogicalName)
		{
			if(!entity.Contains(attributeLogicalName))
			{
				throw new InvalidPluginExecutionException($"attribute = {attributeLogicalName} is not exists");
			}

			var value = entity[attributeLogicalName];
			if (value is T)
			{
				return (T)value;
			}
			else
			{
				throw new InvalidPluginExecutionException($"attribute = {attributeLogicalName} actual type is {value.GetType()} expected type is {typeof(T)}");
			}
		}
	}
}
