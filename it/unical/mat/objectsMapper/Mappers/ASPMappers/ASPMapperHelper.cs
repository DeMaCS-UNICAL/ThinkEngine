public static class ASPMapperHelper
{
    internal static string aspFormat(string hierarchyLevel)
    {
        string clean = "";
        for (int i = 0; i < hierarchyLevel.Length; i++)
        {
            if (char.IsLetterOrDigit(hierarchyLevel[i]))
            {
                clean += hierarchyLevel[i];
            }
        }
        clean = char.ToLower(clean[0]) + clean.Substring(1);
        return clean;

    }
}
