using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Queries.TimeEntries.Requests
{
	public class GetEntryTimeRangeRequest
	{
		public GetEntryTimeRangeRequest(DateTime start, DateTime end, Guid resourceId)
		{
			Start = start;
			End = end;
			ResourceId = resourceId;
		}

		public DateTime Start { get;  }

		public DateTime End { get;  }

		public Guid ResourceId { get; }
}
}
