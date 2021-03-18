using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Metadata.Entities
{
	public static class TimeEntry
	{
		public const string Name = "msdyn_timeentry";

		public static class Properties
		{
			public const string Start = "msdyn_start";
			public const string End = "msdyn_end";
			//public const string Date = "msdyn_date";
			public const string Duration = "msdyn_duration";
			public const string BookableResource = "msdyn_bookableresource";
		}
	}
}
