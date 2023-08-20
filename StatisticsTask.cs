using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace linq_slideviews;

public class StatisticsTask
{
    public static double GetMedianTimePerSlide(List<VisitRecord> visits, SlideType slideType) // U - пользователь, S - slide
    {
        var revelantValues = visits
            .OrderBy(x => x.DateTime)
            .GroupBy(visit => visit.UserId)
            .SelectMany(group => group
                .Bigrams()
                .Where(x => x.First.SlideType == slideType && x.First.SlideId != x.Second.SlideId
                && x.First.UserId.Equals(x.Second.UserId))
                .Select(pair => (pair.Second.DateTime - pair.First.DateTime).TotalMinutes)
                .Where(timeInMin => timeInMin >= 1 && timeInMin <= 120))
            .ToList();

        if (revelantValues.Count == 0)
        {
            return 0.0;
        }

        return revelantValues.Median();
    }
}