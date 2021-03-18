using DynPlugins.Extensions;
using DynPlugins.Metadata.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Data.Converters
{
	public static class TimeEntryDtoConverter
	{
		public static TimeEntryDto ConvertToDto(Entity entity)
		{
			var result = new TimeEntryDto
			{
				Start = entity.GetAttributeValue<DateTime>(TimeEntry.Properties.Start).ToUniversalTime(),
				End = entity.GetAttributeValue<DateTime>(TimeEntry.Properties.End).ToUniversalTime(),
				BookableResource = entity.GetAttributeValue<EntityReference>(TimeEntry.Properties.BookableResource)
			};

			return result;
		}
	}
}
