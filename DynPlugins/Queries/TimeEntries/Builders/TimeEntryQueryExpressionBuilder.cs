using DynPlugins.Extensions;
using DynPlugins.Metadata.Entities;
using DynPlugins.Queries.TimeEntries.Requests;
using DynPlugins.Validations;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Queries.TimeEntries.Builders
{
	public static class TimeEntryQueryExpressionBuilder
	{
		public static QueryExpression Build(GetEntryTimeRangeRequest request)
		{
			request.ArgumentNotNull(nameof(request));

			var query = new QueryExpression(TimeEntry.Name)
			{
				ColumnSet = new ColumnSet(new[]
				{
					TimeEntry.Properties.Start
				}),
				Criteria = new FilterExpression
				{
					Conditions =
					{
						new ConditionExpression
						{
							AttributeName = TimeEntry.Properties.Start,
							Operator = ConditionOperator.Between,
							Values = { request.Start.Date, request.End.EndOfTheDay() }
						},
						new ConditionExpression(TimeEntry.Properties.BookableResource, ConditionOperator.Equal, request.ResourceId)
					}
				}
			};

			return query;
		}
	}
}
