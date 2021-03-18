using DynPlugins.Data.Converters;
using DynPlugins.Extensions;
using DynPlugins.Metadata.Entities;
using DynPlugins.Plugins.TimeEntries;
using DynPlugins.Queries.TimeEntries.Builders;
using DynPlugins.Queries.TimeEntries.Requests;
using DynPlugins.UnitTests.Common;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.UnitTests.PluginTests
{
	[TestClass]
	public class TimeEntryUniqueDatePluginTests
	{
		[TestMethod]
		public void CreationThreeTimeEntriesOnEmptyDbTest()
		{
			XrmFakedContext context = new XrmFakedContext();
			Guid resourseId = Guid.NewGuid();
			DateTime startDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan duration = TimeSpan.FromDays(2);
			var targetTimeEntry = EntityFactory.CreateTimeEntry(startDate, duration, resourseId);
			PreOperationAndThenCreate(context, targetTimeEntry);

			var entities = GetTimeEntries(new GetEntryTimeRangeRequest(startDate, startDate + duration, resourseId), context);
			var timeEntries = entities.Select(e => TimeEntryDtoConverter.ConvertToDto(e)).ToArray();

			Assert.AreEqual(3, timeEntries.Length);

			var firstNewTimeEntry = timeEntries[0];
			Assert.AreEqual(firstNewTimeEntry.BookableResource.Id, resourseId);
			Assert.AreEqual(firstNewTimeEntry.Start, startDate.AddDays(1));
			Assert.AreEqual(firstNewTimeEntry.End, startDate.AddDays(1).EndOfTheDay());
			var secondNewTimeEntry = timeEntries[1];
			Assert.AreEqual(secondNewTimeEntry.BookableResource.Id, resourseId);
			Assert.AreEqual(secondNewTimeEntry.Start, startDate.AddDays(2));
			Assert.AreEqual(secondNewTimeEntry.End, startDate.AddDays(2).EndOfTheDay());
			// the "target" TimeEntry is created last
			var targetEntry = timeEntries[2];
			Assert.AreEqual(targetEntry.BookableResource.Id, resourseId);
			Assert.AreEqual(targetEntry.Start, startDate);
			Assert.AreEqual(targetEntry.End, targetEntry.Start.EndOfTheDay());
		}

		[TestMethod]
		public void DataDublicationIsNotAllowedTest()
		{
			XrmFakedContext context = new XrmFakedContext();
			var plugin = new TimeEntryUniqueDatePlugin();
			Guid resourseId = Guid.NewGuid();
			DateTime startDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan duration = TimeSpan.FromDays(1);

			context.Initialize(new List<Entity> {
				EntityFactory.CreateTimeEntry(startDate, TimeSpan.FromHours(2), resourseId),
				EntityFactory.CreateTimeEntry(startDate.AddDays(1), TimeSpan.FromHours(2), resourseId)
			});

			var timeEntry = EntityFactory.CreateTimeEntry(startDate, duration, resourseId);

			Assert.ThrowsException<InvalidPluginExecutionException>(() => context.ExecutePluginWithTarget(plugin, timeEntry, "Create", 20),
				"Duplicates creation is canceled");
		}

		[TestMethod]
		public void CreationOnlyNonExistingTimeEntriesTest()
		{
			XrmFakedContext context = new XrmFakedContext();
			Guid resourseId = Guid.NewGuid();
			DateTime startDate = new DateTime(2000, 1, 1, 9, 0, 0, DateTimeKind.Utc);
			TimeSpan duration = TimeSpan.FromDays(2);

			var targetTimeEntry = EntityFactory.CreateTimeEntry(startDate, duration, resourseId);

			var existingEntryStart = startDate.AddDays(1);
			var existingEntryDuration = TimeSpan.FromHours(2);
			context.Initialize(new List<Entity> {
				EntityFactory.CreateTimeEntry(existingEntryStart, existingEntryDuration, resourseId),
			});

			var request = new GetEntryTimeRangeRequest(startDate, startDate + duration, resourseId);

			var entitiesBeforeCreation = GetTimeEntries(request, context);
			Assert.AreEqual(1, entitiesBeforeCreation.Count);
			// One TimeEnrty exists, 1 should be added before existing one and 1 should be added after existing one
			PreOperationAndThenCreate(context, targetTimeEntry);

			var entitiesAfterCreation = GetTimeEntries(request, context);
			var timeEntries = entitiesAfterCreation.Select(e => TimeEntryDtoConverter.ConvertToDto(e)).ToArray();
			Assert.AreEqual(3, timeEntries.Length);

			var existingTimeEntry = timeEntries[0];
			Assert.AreEqual(existingTimeEntry.BookableResource.Id, resourseId);
			Assert.AreEqual(existingTimeEntry.Start, existingEntryStart);
			Assert.AreEqual(existingTimeEntry.End, existingEntryStart + existingEntryDuration);

			var firstNewTimeEntry = timeEntries[1];
			Assert.AreEqual(firstNewTimeEntry.BookableResource.Id, resourseId);
			Assert.AreEqual(firstNewTimeEntry.Start, startDate.Date.AddDays(2));
			Assert.AreEqual(firstNewTimeEntry.End, startDate.Date.AddDays(2).EndOfTheDay());

			// a new time entry before the existing one is a changed "target" TimeEntry
			// the "target" TimeEntry is created last
			var secondNewTimeEntry = timeEntries[2];
			Assert.AreEqual(secondNewTimeEntry.BookableResource.Id, resourseId);
			Assert.AreEqual(secondNewTimeEntry.Start, startDate);
			Assert.AreEqual(secondNewTimeEntry.End, startDate.EndOfTheDay());
		}

		/// <summary>
		/// Imitation of work through the UI
		/// </summary>
		/// <param name="context"></param>
		/// <param name="timeEntry"></param>
		private void PreOperationAndThenCreate(XrmFakedContext context, Entity timeEntry)
		{
			var plugin = new TimeEntryUniqueDatePlugin();
			context.ExecutePluginWithTarget(plugin, timeEntry, "Create", 20);
			context.GetOrganizationService().Create(timeEntry);
		}

		private DataCollection<Entity> GetTimeEntries(GetEntryTimeRangeRequest request, XrmFakedContext context)
		{
			var queryExpression = new QueryExpression(TimeEntry.Name)
			{
				ColumnSet = new ColumnSet(new[]
				{
					TimeEntry.Properties.Duration,
					TimeEntry.Properties.BookableResource,
					TimeEntry.Properties.Start,
					TimeEntry.Properties.End
				}),
				Criteria = new FilterExpression
				{
					Conditions =
					{
						new ConditionExpression
						{
							AttributeName = TimeEntry.Properties.Start,
							Operator = ConditionOperator.Between,
							Values = { request.Start.Date, request.End.Date.EndOfTheDay() }
						},
						new ConditionExpression(TimeEntry.Properties.BookableResource, ConditionOperator.Equal, request.ResourceId)
					}
				}
			};
			return context.GetOrganizationService().RetrieveMultiple(queryExpression).Entities;
		}
	}
}
