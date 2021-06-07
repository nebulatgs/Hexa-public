using System.Collections.Generic;

public static class AllowList
{
    private static List<string> Allowed = new()
    {
        "lesbian",
        "lesbians"
    };
    public static void AddAllowed(ProfanityFilter.ProfanityFilter filter)
    {
        Allowed.ForEach(x => filter.AllowList.Add(x));
    }
}