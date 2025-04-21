using NodaTime;

namespace ChoreoHelper.InternetTime;

public static class SwatchInternetTimeCalculator
{
    public static int Calculate(Instant utcTimestamp)
    {
        // Define the BMT (Biel Mean Time) zone as UTC+1
        DateTimeZone bmtZone = DateTimeZone.ForOffset(Offset.FromHours(1));

        // Convert the UTC instant to BMT
        ZonedDateTime bmtTime = utcTimestamp.InZone(bmtZone);
        
        // Calculate total seconds in the day
        int totalSeconds = bmtTime.Hour * 3600 + bmtTime.Minute * 60 + bmtTime.Second;

        // Calculate .beats (1 beat = 86.4 seconds)
        double beats = totalSeconds / 86.4;

        // Return rounded .beats as an integer
        return (int)Math.Floor(beats);
    }
}