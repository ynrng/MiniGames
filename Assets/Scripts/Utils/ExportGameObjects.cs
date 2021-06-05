/* https://docs.unity3d.com/Packages/com.unity.formats.fbx@2.0/manual/devguide.html */
using System.IO;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.Formats.Fbx.Exporter;
#endif

public class Utils
{

#if UNITY_EDITOR

    public static void ExportGameObject(Object obj, string name)
    {
        string filePath = Path.Combine(Application.dataPath, "Prefabs", name + ".fbx");
        ModelExporter.ExportObject(filePath, obj);

        // ModelExporter.ExportObject can be used instead of
        // ModelExporter.ExportObjects to export a single game object
    }
#endif

}
