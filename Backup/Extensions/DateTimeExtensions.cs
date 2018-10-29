using System;
namespace SagicorNow.Extensions
{
    public static class DateTimeExtensions
    {
		// This method calculates the users age
		public static Int32 GetAge(this DateTime dateOfBirth)
		{
			var today = DateTime.Today;
			var a = (today.Year * 100 + today.Month) * 100 + today.Day;
			var b = (dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day;
			return (a - b) / 10000;
		}
    }
}
