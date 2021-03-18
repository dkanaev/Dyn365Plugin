using DynPlugins.Metadata.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Extensions
{
	public static class TimeEntryEntityExtension
	{
		public static void SetDuration(this Entity entity, TimeSpan duration)
		{
			entity[TimeEntry.Properties.Duration] = (int)duration.TotalMinutes;
		}

		public static void SetStart(this Entity entity, DateTime start)
		{
			entity[TimeEntry.Properties.Start] = start.ToUniversalTime();
		}

		public static void SetEnd(this Entity entity, DateTime end)
		{
			entity[TimeEntry.Properties.End] = end.ToUniversalTime();
		}

		public static void SetBookableResource(this Entity entity, Guid id)
		{
			entity[TimeEntry.Properties.BookableResource] = new EntityReference(BookableResource.Name, id);
		}
	}
}
