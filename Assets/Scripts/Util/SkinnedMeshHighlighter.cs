using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshHighlighter : MonoBehaviour
{
    public List<SkinnedMeshRenderer> meshesToHighlight;
    public Material originalMaterial;
    public Material highlightedMaterial;

    public void HighlightMesh(bool highlight)
    {
        foreach (var mesh in meshesToHighlight)
        {
            mesh.material = (highlight)? highlightedMaterial : originalMaterial;
        }
    }
}
