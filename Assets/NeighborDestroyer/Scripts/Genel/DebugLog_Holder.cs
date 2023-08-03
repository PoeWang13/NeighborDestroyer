using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DebugLog Holder", fileName = "Debug Log Holder")]
public class DebugLog_Holder : ScriptableObject
{
    [SerializeField] private List<DebugList> debugs = new List<DebugList>();

    public bool CanShowDebug(string title)
    {
        for (int e = 0; e < debugs.Count; e++)
        {
            if (debugs[e].title == title)
            {
                return debugs[e].showDebug;
            }
        }
        debugs.Add(new DebugList(title));
        return true;
    }
}