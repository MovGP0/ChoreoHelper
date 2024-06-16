using System.Diagnostics;
using System.Reflection;

namespace ChoreoHelper.Database;

public static class DatabasePathHelper
{
    private static string _path = string.Empty;

    public static string GetPathToDatabase()
    {
        if (_path != string.Empty) return _path;

        var baseFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Debug.Assert(baseFolder != null);
        _path = Path.Combine(baseFolder, "dance transitions.sliccdb");
        return _path;
    }
}