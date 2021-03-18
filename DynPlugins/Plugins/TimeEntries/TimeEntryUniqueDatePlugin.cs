using Microsoft.Xrm.Sdk;
using System;
using DynPlugins.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynPlugins.Metadata.Entities;
using Microsoft.Xrm.Sdk.Query;
using DynPlugins.Queries.TimeEntries.Builders;
using DynPlugins.Queries.TimeEntries.Requests;
using DynPlugins.Data.Converters;
using DynPlugins.Data;

namespace DynPlugins.Plugins.TimeEntries
{
	public class TimeEntryUniqueDatePlugin : IPlugin
	{
		public void Execute(IServiceProvider serviceProvider)
		{
			// https://docs.microsoft.com/ru-ru/powerapps/developer/data-platform/best-practices/business-logic/develop-iplugin-implementations-stateless#problematic-patterns
			// to avoid problematic pattern should create pluginExecutionFacade and pass it everywhere (instead of using a variable of class)
			var pluginExecutionFacade = new PluginExecutionFacade(serviceProvider);
			try
			{
				var context = pluginExecutionFacade.Context;
				// if another plugin creates TimeEntry then this check should be removed
				if (context.Depth > 1)
				{
					return;
				}
				var targetEntity = context.InputParameters.GetTargetEntity();
				var targetTimeEntryDto = TimeEntryDtoConverter.ConvertToDto(targetEntity);

				if (targetTimeEntryDto.End.Date <= targetTimeEntryDto.Start.Date)
				{
					return;
				}
				// the main logic is located here for clarity
				var existingTimeEntrieDates = GetExistingTimeEntryDates(pluginExecutionFacade, targetTimeEntryDto);
				var date = targetTimeEntryDto.Start.Date;
				var maxDate = targetTimeEntryDto.End.Date;
				var datesToBeCreated = new List<DateTime>();
				while (date <= maxDate)
				{
					if (!existingTimeEntrieDates.Contains(date))
					{
						datesToBeCreated.Add(date);
					}
					date = date.AddDays(1);
				}

				if (!datesToBeCreated.Any())
				{
					throw new InvalidPluginExecutionException("Duplicates creation is canceled");
				}

				// target TimeEntry should be changed
				var firstDateToBeCreated = datesToBeCreated[0];
				if (targetTimeEntryDto.Start.Date == firstDateToBeCreated)
				{
					var duration = targetTimeEntryDto.Start.EndOfTheDay() - targetTimeEntryDto.Start;
					targetEntity.SetDuration(duration);
					targetEntity.SetEnd(targetTimeEntryDto.Start + duration);
				}
				else
				{
					var duration = TimeSpan.FromDays(1) - TimeSpan.FromMinutes(1);
					targetEntity.SetStart(firstDateToBeCreated);
					targetEntity.SetDuration(duration);
					targetEntity.SetEnd(firstDateToBeCreated + duration);
				}
				// the first TimeEntry will be created later as the "target" TimeEntry
				datesToBeCreated.RemoveAt(0);

				foreach (var dateItem in datesToBeCreated)
				{
					var duration = (TimeSpan.FromDays(1) - TimeSpan.FromMinutes(1));
					var newEntity = targetEntity.Clone();
					newEntity.SetStart(dateItem);
					newEntity.SetDuration(duration);
					newEntity.SetEnd(dateItem + duration);
					pluginExecutionFacade.OrganizationService.Create(newEntity);
				}
			}
			catch (Exception ex)
			{
				pluginExecutionFacade.TracingService.Trace("{0}: {1}", nameof(TimeEntryUniqueDatePlugin), ex.ToString());
				// just to debug
				throw new InvalidPluginExecutionException($"An error occurred in {nameof(TimeEntryUniqueDatePlugin)}.", ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pluginExecutionFacade"></param>
		/// <param name="newTimeEntryDto"></param>
		/// <returns></returns>
		private IEnumerable<DateTime> GetExistingTimeEntryDates(PluginExecutionFacade pluginExecutionFacade, TimeEntryDto newTimeEntryDto)
		{
			var queryExpression = TimeEntryQueryExpressionBuilder.Build(new GetEntryTimeRangeRequest(
				start: newTimeEntryDto.Start,
				end: newTimeEntryDto.End,
				resourceId: newTimeEntryDto.BookableResource.Id));
			var service = pluginExecutionFacade.OrganizationService;
			var entityCollection = service.RetrieveMultiple(queryExpression);
			return entityCollection.Entities.Select(e => e.GetAttributeValue<DateTime>(TimeEntry.Properties.Start).ToUniversalTime().Date);
		}
	}
}
