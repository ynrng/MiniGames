
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DrawCube : DrawCylinder
{
    public Material normalMaterial;
    public Material highlightMaterial;

    // -------------private--------------------
    // private MeshFilter meshF;
    private MeshRenderer meshR;

    [SerializeField] private Material[] conbinedMaterial;
    // private Vector3[] vertices;

    private new void Awake()
    {
        base.Awake();
        meshR = gameObject.GetComponent<MeshRenderer>();
    }
    void Start()
    {
        segmentsHeight = 1;
        segmentsRadial = 4;
        radiusRound = Mathf.Sqrt(2) / 2f;
        angleInitial = Mathf.PI / 4;
        useSubmesh = true;
        sideOnly = false;

        conbinedMaterial = new Material[segmentsRadial + (sideOnly ? 0 : 2)];
        for (int i = 0; i < conbinedMaterial.Length; i++)
        {
            conbinedMaterial[i] = normalMaterial;
        }
        meshR.materials = conbinedMaterial;

        Draw();
    }

    public void UpdateFace(int index, bool useHighlight)
    {
        conbinedMaterial[index] = useHighlight ? highlightMaterial : normalMaterial;
    }
}
