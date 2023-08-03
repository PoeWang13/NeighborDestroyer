using System.IO;
using UnityEditor;

public class DebugLogger_Editor : EditorWindow
{
    /// <summary>
    /// Create Debug Logger Scriptable object and folders.
    /// </summary>
    [MenuItem("Tools/Create Debug Logger")]
    public static void CreateDebugLogger()
    {
        #region Create Folder For Debug Logger
        // Resources folder control
        if (!Directory.Exists("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        // Resources/Scriptable folder control
        if (!Directory.Exists("Assets/Resources/Scriptable"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Scriptable");
        }
        // Resources/Scriptable/Genel folder control
        if (!Directory.Exists("Assets/Resources/Scriptable/Genel"))
        {
            AssetDatabase.CreateFolder("Assets/Resources/Scriptable", "Genel");
        }
        #endregion

        #region Create Debug Logger
        DebugLog_Holder debug_Holder = CreateInstance<DebugLog_Holder>();
        AssetDatabase.CreateAsset(debug_Holder, "Assets/Resources/Scriptable/Genel/Debug_Holder.asset");
        #endregion

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}