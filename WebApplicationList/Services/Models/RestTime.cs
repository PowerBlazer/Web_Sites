namespace WebApplicationList.Services.Models
{
    public class RestTime
    {
        public string GetRest(DateTime first, DateTime second)
        {
            var timeSpan = first - second;

            if (timeSpan.Days >= 1)
            {
                if (timeSpan.Days < 2)
                {
                    return Convert.ToInt32(timeSpan.Days) + " день назад";
                }

                if (timeSpan.Days >= 2 && timeSpan.Days < 5)
                    return Convert.ToInt32(timeSpan.Days) + " дня назад";

                return Convert.ToInt32(timeSpan.Days) + " дней назад";
            }

            if (timeSpan.Hours >= 1)
            {
                if (timeSpan.Hours < 2)
                {
                    return Convert.ToInt32(timeSpan.Hours) + " час назад";
                }

                if(timeSpan.Hours >=2 && timeSpan.Hours<5)
                    return Convert.ToInt32(timeSpan.Hours) + " часа назад";

                return Convert.ToInt32(timeSpan.Hours) + " часов назад";
            }

            if (timeSpan.Minutes >= 1)
            {
                if (timeSpan.Minutes < 2)
                {
                    return Convert.ToInt32(timeSpan.Days) + " минута назад";
                }

                if (timeSpan.Minutes >= 2 && timeSpan.Minutes < 5)
                    return Convert.ToInt32(timeSpan.Minutes) + " минуты назад";

                return Convert.ToInt32(timeSpan.Minutes) + " минут назад";
            }

            if (timeSpan.Seconds >= 1)
            {
                if (timeSpan.Seconds < 2)
                {
                    return Convert.ToInt32(timeSpan.Seconds) + " секунду назад";
                }

                if (timeSpan.Seconds >= 2 && timeSpan.Seconds < 5)
                    return Convert.ToInt32(timeSpan.Seconds) + " секунды назад";

                return Convert.ToInt32(timeSpan.Seconds) + " секунд назад";
            }

            return "недавно";
        }
    }
}
