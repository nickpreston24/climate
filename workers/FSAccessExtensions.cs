namespace climate;

/// <summary>
/// Extensions for Accessibility to files
/// </summary>
internal static class FSAccessExtensions
{
    public class File
    {
        public static bool TryReadAllText(
            string filepath
            , out string text
            , bool should_throw_on_error = false
            , string fallback = ""
        )
        {
            try
            {
                text = System.IO.File.ReadAllText(filepath);
                return true;
            }
            catch (Exception e)
            {
                //todo: use dev's logger instead.
                // Console.WriteLine(e);
                if (should_throw_on_error)
                    throw;
            }

            text = fallback;

            return false;
        }
    }

    public static bool IsFileAccessible(string filePath, FileAccess access)
    {
        try
        {
            // Attempt to open the file with the specified access and no sharing.
            // This will throw an exception if the file is in use or permissions are insufficient.
            using (FileStream stream = new FileStream(filePath, FileMode.Open,
                       access, FileShare.None))
            {
                // If the stream is successfully opened, the file is accessible.
                return true;
            }
        }
        catch (FileNotFoundException)
        {
            // The file does not exist.
            return false;
        }
        catch (IOException)
        {
            // The file is in use by another process or there are other I/O errors.
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            // The current user does not have the necessary permissions.
            return false;
        }
        catch (Exception)
        {
            // Catch any other unexpected exceptions.
            return false;
        }
    }

    public static bool IsFileAccessible(this FileInfo fileInfo)
    {
        try
        {
            // Attempt to open the file for reading with no sharing allowed.
            // If another process has an exclusive lock, this will throw an IOException.
            using (FileStream stream = fileInfo.Open(FileMode.Open,
                       FileAccess.Read, FileShare.None))
            {
                // If the stream is successfully opened, the file is accessible for reading.
                return true;
            }
        }
        catch (IOException)
        {
            // An IOException indicates the file is likely in use or inaccessible.
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            // This exception indicates permission issues.
            return false;
        }
        catch (Exception)
        {
            // Catch other potential exceptions during file access.
            return false;
        }
    }

    public static FileInfo AsFileInfo(this string filepath) =>
        new FileInfo(filepath);
}