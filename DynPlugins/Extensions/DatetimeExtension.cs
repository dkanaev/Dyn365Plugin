using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynPlugins.Extensions
{
	public static class DatetimeExtension
	{
		public static DateTime EndOfTheDay(this DateTime date)
		{
			return date.Date.AddDays(1).AddMinutes(-1);
		}
	}
}
