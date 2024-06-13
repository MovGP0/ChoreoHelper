using System.Diagnostics;
using System.Reflection;

namespace ChoreoHelper.Database;

public static class DatabasePathHelper
{
    private static string path = string.Empty;

    public static string GetPathToDatabase()
    {
        if (path != string.Empty) return path;

        var baseFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Debug.Assert(baseFolder != null);
        path = Path.Combine(baseFolder, "dance transitions.sliccdb");
        return path;
    }
}