using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Inspector : MonoBehaviour
{
    public Vector3[] vertices;
    public int[] triangles;
    // public Vector3 tans;
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
