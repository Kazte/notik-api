namespace NotesApi.Utilities;

public class GuidUtilities
{
    public static string GenerateGuid()
    {
        return Guid.NewGuid().ToString("n");
    }
}