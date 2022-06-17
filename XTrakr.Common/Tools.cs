namespace XTrakr.Common;
public static class Tools
{
    public static bool Any(params bool[] flags) => flags.Any(x => x);

    public static bool All(params bool[] flags) => flags.All(x => x);

    public static T? GreatestOf<T>(params T[] items) where T : IComparable<T> => items.Max(x => x);

    public static T? LeastOf<T>(params T[] items) where T : IComparable<T> => items.Min(x => x);
}
