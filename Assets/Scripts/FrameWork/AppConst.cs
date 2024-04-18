

public enum GameMode
{
    EditorMode,
    PackageBundle,
    UpdateMode
}

public class AppConst
{
    public static string FileListName = "file.txt";

    public static GameMode GameMode = GameMode.EditorMode;

    //public const string ResourceUrl = "http://47.120.62.182/AssetBundles";
    public const string ResourceUrl = "http://127.0.0.1:58302/AssetBundles";
}