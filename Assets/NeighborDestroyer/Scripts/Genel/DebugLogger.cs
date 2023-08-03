using UnityEngine;

public enum DebugType
{
    Log, Warning, Error
}
public enum DebugColor
{
    red, white, black, blue, yellow, green, grey, gray, magenta, cyan
}
[System.Serializable]
public class DebugList
{
    public string title;
    public bool showDebug = true;
    public DebugList(string title)
    {
        this.title = title;
    }
}
public class DebugLogger : MonoBehaviour
{
    public static DebugLog_Holder debugLog_Holder;

    /// <summary>
    /// For using example
    /// </summary>
    //private void Start()
    //{
    //    DebugLogger.Debuglog("You don't have a Holder.", "Debug Holder", DebugColor.red, DebugType.Error);
    //}
    private void Awake()
    {
        debugLog_Holder = Resources.Load<DebugLog_Holder>("Debug Log Holder");
        if (debugLog_Holder == null)
        {
            Debug.LogError("<color=red>Debug Holder : You don't have a Holder.</color>");
        }
    }
    /// <summary>
    /// Function to send debugs from one place.
    /// </summary>
    /// <param name="message">The message to be debugged.</param>
    /// <param name="title">The header of the message to be debugged.</param>
    /// <param name="debugColor">The color of the title to be debugged.</param>
    /// <param name="debugType">It is the type of Debug.</param>
    public static void Debuglog(string message, string title = "", DebugColor debugColor = DebugColor.grey, DebugType debugType = DebugType.Log)
    {
        if (debugLog_Holder.CanShowDebug(title))
        {
            if (debugType == DebugType.Log)
            {
                Debug.Log("<color=" + debugColor.ToString() + ">" + title + "</color> : " + message);
            }
            else if (debugType == DebugType.Warning)
            {
                Debug.LogWarning("<color=" + debugColor.ToString() + ">" + title + "</color> : " + message);
            }
            else if (debugType == DebugType.Error)
            {
                Debug.LogError("<color=" + debugColor.ToString() + ">" + title + "</color> : " + message);
            }
        }
    }
}