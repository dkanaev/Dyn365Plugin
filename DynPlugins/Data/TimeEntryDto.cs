using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Data
{
	public class TimeEntryDto
	{
		public DateTime Start { get; set; }

		public DateTime End { get; set; }

		public EntityReference BookableResource { get; set; }
	}
}
