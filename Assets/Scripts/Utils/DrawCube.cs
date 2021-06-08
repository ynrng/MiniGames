using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DrawCube : DrawCylinder
{
    [Header("Cube Settings")]
    public Material normalMaterial;
    public Material highlightMaterial;

    // -------------private--------------------
    // private MeshFilter meshF;
    private MeshRenderer meshR;

    private Material[] conbinedMatsNormal;
    // private Vector3[] vertices;

    private new void Awake()
    {
        base.Awake();
        meshR = gameObject.GetComponent<MeshRenderer>();

        segmentsHeight = 1;
        segmentsRadial = 4;
        radiusRound = Mathf.Sqrt(2) / 2f;
        angleInitial = Mathf.PI / 4;
        useSubmesh = true;
        sideOnly = false;

        conbinedMatsNormal = new Material[segmentsRadial + (sideOnly ? 0 : 2)];
        for (int i = 0; i < conbinedMatsNormal.Length; i++)
        {
            conbinedMatsNormal[i] = normalMaterial;
        }
    }

    void Start()
    {
        Draw();
        meshR.materials = conbinedMatsNormal;
    }

    public void UpdateFace(int index, bool useHighlight)
    {
        conbinedMatsNormal[index] = useHighlight ? highlightMaterial : normalMaterial;
        meshR.materials = conbinedMatsNormal;
    }

    public void ResetFace()
    {
        for (int i = 0; i < conbinedMatsNormal.Length; i++)
        {
            conbinedMatsNormal[i] = normalMaterial;
        }
        meshR.materials = conbinedMatsNormal;
    }
}
