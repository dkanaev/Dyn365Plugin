using DynPlugins.Extensions;
using DynPlugins.Metadata.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.UnitTests.Common
{
	internal static class EntityFactory
	{
		internal static Entity CreateTimeEntry(DateTime start, TimeSpan duration, Guid resourceId)
		{
			var result = new Entity(TimeEntry.Name)
			{
				Id = Guid.NewGuid()
			};
			result.SetStart(start);
			result.SetDuration(duration);
			result.SetEnd(start + duration);
			result.SetBookableResource(resourceId);
			return result;
		}
	}
}
