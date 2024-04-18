using System.IO;
public class FileUtility
{

    public static bool IsExits(string fileName)
    {
        FileInfo file = new FileInfo(fileName);
        return file.Exists;
    }

    public static void WriteFile(string Path, byte[] data)
    {
        Path = PathUtility.GetStandPath(Path);

        string subDir = Path.Substring(0, Path.LastIndexOf("/"));

        if (Directory.Exists(subDir) == false)
        {
            Directory.CreateDirectory(subDir);
        }
        FileInfo file = new FileInfo(Path);
        if (file.Exists)
        {
            file.Delete();
        }

        using (FileStream stream = new FileStream(Path, FileMode.Create, FileAccess.Write))
        {
            stream.Write(data, 0, data.Length);
        }

    }

}