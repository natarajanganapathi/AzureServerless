namespace Incubation.AzConf.Common;
public static class UniqueRandom
{
    public static IEnumerable<Int32> Get(int count, int max, int min = 0)
    {
        var random = new Random();
        Dictionary<int, bool> dict = new Dictionary<int, bool>();
        while (dict.Count < count)
        {
            var val = random.Next(min, max);
            if (!dict.ContainsKey(val))
            {
                dict.Add(val, true);
            }
        }
        return dict.Keys.ToList();
    }
}