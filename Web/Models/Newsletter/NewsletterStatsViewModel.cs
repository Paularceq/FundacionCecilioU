namespace Web.Models.Newsletter
{
    public class NewsletterStatsViewModel
    {
        public int TotalSubscriptions { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int InactiveSubscriptions { get; set; }
        public int DailySubscriptions { get; set; }
        public int WeeklySubscriptions { get; set; }
        public int MonthlySubscriptions { get; set; }
        public int GrowthThisMonth { get; set; }
        public int TotalNewsletters { get; set; }
        public int NewslettersSent { get; set; }

        // Propiedades calculadas
        public double ActivePercentage => TotalSubscriptions > 0
            ? Math.Round((double)ActiveSubscriptions / TotalSubscriptions * 100, 1)
            : 0;

        public double GrowthPercentage => TotalSubscriptions > 0
            ? Math.Round((double)GrowthThisMonth / TotalSubscriptions * 100, 1)
            : 0;
    }
}