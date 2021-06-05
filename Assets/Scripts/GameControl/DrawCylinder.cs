using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DrawCylinder : MonoBehaviour
{
    // -------------public--------------------
    // how many segments do we want in between;
    /// <summary>高有几层</summary>
    public int segmentsHeight = 1; //
    /// <summary>底边顶点数</summary>
    public int segmentsRadial = 4;
    /// <summary>顶点是否重用</summary>
    public bool isDiscrete = true;
    /// <summary>底边半径</summary>
    public float radiusRound = 1f;

    /// <summary>第一个顶点偏移x轴的角度</summary>
    public float angleInitial = 0;
    /// <summary>是否把每个面切成submesh, currently only work with isDiscrete=true</summary>
    public bool useSubmesh = false;
    /// <summary>只画侧面, currently only work with isDiscrete=true</summary>
    public bool sideOnly = true;

    // -------------private--------------------
    [SerializeField] private List<Vector3> vertices;
    [SerializeField] private List<int> triangles;
    [SerializeField] private List<int>[] trianglesSub;
    private Mesh mesh;

    protected void Awake()
    {
        // to avoid add mesh multiple times; declare at top require;
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Draw();
    }

    private void makeMeshData()
    {

        float tranferToCenterY = -segmentsHeight / 2f;

        //
        //prepare 平面上的点的位置，以原点为中心等分半径1的圆
        List<Vector3> xzs = new List<Vector3>(segmentsRadial + 1);
        // from 0 to pi,zz
        float stepR = Mathf.PI * 2 / segmentsRadial;
        for (int i = 0; i < segmentsRadial; i++)
        {
            float angle = stepR * i + angleInitial;
            xzs.Add(new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radiusRound);
        }
        //connect the first with last
        //add back origin point so in for loop it dose not overflow
        xzs.Add(xzs[0]);

        //
        //start generation
        vertices = new List<Vector3>();
        //faces: basicly from 0 to segmentsHeight*segmentsRound-1
        int sidetrianglesCount = (segmentsHeight * segmentsRadial) * 6;

        if (useSubmesh)
        {
            trianglesSub = new List<int>[segmentsRadial + (sideOnly ? 0 : 2)];
        }
        else
        {
            triangles = new List<int>();
        }

        if (isDiscrete)
        {
            //faces: basicly from 0 to segmentsHeight*segmentsRound-1
            if (!useSubmesh)
            {
                triangles = Enumerable.Range(0, sidetrianglesCount).ToList();
            }

            // only need to generate side
            for (int j = 0; j < segmentsHeight; j++)
            {
                for (int i = 0; i < segmentsRadial; i++)
                {
                    // one face : order matters. points
                    vertices.Add(xzs[i] + Vector3.up * (j + tranferToCenterY));
                    vertices.Add(xzs[i] + Vector3.up * (j + 1 + tranferToCenterY));
                    vertices.Add(xzs[i + 1] + Vector3.up * (j + tranferToCenterY));

                    vertices.Add(xzs[i + 1] + Vector3.up * (j + tranferToCenterY));
                    vertices.Add(xzs[i] + Vector3.up * (j + 1 + tranferToCenterY));
                    vertices.Add(xzs[i + 1] + Vector3.up * (j + 1 + tranferToCenterY));

                    if (useSubmesh)
                    {
                        if (trianglesSub[i] == null) { trianglesSub[i] = new List<int>(); }
                        trianglesSub[i].AddRange(Enumerable.Range(6 * (j * segmentsRadial + i), 6));
                    }
                }
            }

            if (!sideOnly)
            {
                // generate top and bottom triangles

                if (useSubmesh)
                {
                    trianglesSub[segmentsRadial] = Enumerable.Range(sidetrianglesCount, segmentsRadial * 3).ToList();//bottom
                    trianglesSub[segmentsRadial + 1] = Enumerable.Range(sidetrianglesCount + segmentsRadial * 3, segmentsRadial * 3).ToList();//top
                }
                else
                {
                    triangles.AddRange(Enumerable.Range(sidetrianglesCount, 2 * segmentsRadial * 3));
                }

                // bottom face
                Vector3 bottomCenterY = Vector3.up * tranferToCenterY;
                for (int i = 0; i < segmentsRadial; i++)
                {
                    // one face : order matters. points
                    vertices.Add(bottomCenterY);
                    vertices.Add(xzs[i] + bottomCenterY);
                    vertices.Add(xzs[i + 1] + bottomCenterY);

                }

                // top face
                bottomCenterY *= -1;
                for (int i = 0; i < segmentsRadial; i++)
                {
                    // one face : order matters. points
                    vertices.Add(bottomCenterY);
                    vertices.Add(xzs[i + 1] + bottomCenterY);
                    vertices.Add(xzs[i] + bottomCenterY);

                }

            }
        }
        else
        {
            // only need to generate side
            for (int j = 0; j < segmentsHeight + 1; j++)
            {
                for (int i = 0; i < segmentsRadial; i++)
                {
                    //points, order matters
                    vertices.Add(xzs[i] + Vector3.up * (j + tranferToCenterY));
                }
            }

            // only need to generate side
            for (int j = 0; j < segmentsHeight; j++)
            {
                for (int i = 0; i < segmentsRadial; i++)
                {
                    //points. order matters
                    int indexk = j * segmentsRadial;
                    int iplus1 = (i + 1) % segmentsRadial; //for the last quad to reconnect to the 1st

                    triangles.Add(indexk + i);
                    triangles.Add(indexk + i + segmentsRadial);
                    triangles.Add(indexk + iplus1);//i + 1

                    triangles.Add(indexk + iplus1);
                    triangles.Add(indexk + i + segmentsRadial);
                    triangles.Add(indexk + iplus1 + segmentsRadial);
                }

            }
        }

    }

    private void createMesh()
    {

        // if (addStandardMaterial) {
        //     MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //     meshRenderer.material = new Material(Shader.Find("Standard"));
        // }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        if (useSubmesh)
        {
            mesh.subMeshCount = trianglesSub.Length;
            for (int i = 0; i < trianglesSub.Length; i++)
            {
                mesh.SetTriangles(trianglesSub[i], i);
            }
        }
        else
        {
            mesh.triangles = triangles.ToArray();

        }

        mesh.RecalculateNormals();
        // gameObject.transform.localScale = Vector3.one * 1000;
    }

    // -------------public--------------------

    public void Draw()
    {
        makeMeshData();
        createMesh();
    }

    public List<int>[] groupSamePoint()
    {
        List<int>[] points = new List<int>[(segmentsHeight + 1) * segmentsRadial];
        // first is self;
        // points[0] = airplane;
        for (int j = 0; j <= segmentsHeight; j++)
        {
            for (int i = 0; i < segmentsRadial; i++)
            {
                int indexk = j * segmentsRadial + i;
                int zeroBalance = i % segmentsRadial == 0 ? segmentsRadial : 0;
                points[indexk] = new List<int>();
                // mind edge case
                if (j != segmentsHeight)
                {
                    // top line
                    points[indexk].Add((indexk + zeroBalance) * 6 - 3); // t r t
                    points[indexk].Add((indexk + zeroBalance) * 6 - 4); // t r t
                    points[indexk].Add((indexk) * 6); // t r b
                }
                if (j != 0)
                {
                    // bottome line
                    points[indexk].Add((indexk - segmentsRadial) * 6 + 1); // b r
                    points[indexk].Add((indexk - segmentsRadial) * 6 + 4); // b r
                    points[indexk].Add((indexk - segmentsRadial + zeroBalance) * 6 - 1); // b r
                }
            }
        }
        return points;
    }

}
