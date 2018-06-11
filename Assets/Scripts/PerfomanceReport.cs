using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PerfomanceReport {

    private static List<float> chunkTimes = new List<float>();

    public static System.DateTime firstTime;

    public static float getMin()
    {
        float min = 0;

        min = chunkTimes.Min();

        return min;
    }

    public static float getMax()
    {
        float max = 0;

        max = chunkTimes.Max();

        return max;
    }

    public static float getAvg()
    {
        float avg = 0;

        avg = chunkTimes.Average();

        return avg;
    }

    public static float getTotal()
    {
        float total = 0;

        total = ((float)(System.DateTime.Now - firstTime).TotalMilliseconds);

        return total;
    }

    public static void addTimeToList(float time)
    {
        chunkTimes.Add(time);
    }
}
