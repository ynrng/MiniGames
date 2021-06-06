using UnityEngine;

public static class Utils
{

    public static void rotateRigidBodyAroundPointBy(Rigidbody rb, Vector3 origin, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);
        rb.MovePosition(q * (rb.transform.position - origin) + origin);
        rb.MoveRotation(rb.transform.rotation * q);
    }

    //     /* https://docs.unity3d.com/Packages/com.unity.formats.fbx@2.0/manual/devguide.html */
    // #if UNITY_EDITOR

    //     public static void ExportGameObject(Object obj, string name)
    //     {
    //         string filePath = Path.Combine(Application.dataPath, "Prefabs", name + ".fbx");
    //         ModelExporter.ExportObject(filePath, obj);

    //         // ModelExporter.ExportObject can be used instead of
    //         // ModelExporter.ExportObjects to export a single game object
    //     }
    // #endif

}
