using UnityEngine;
using System.Collections.Generic;

public class MapMeshCombiner : MonoBehaviour
{
    private const int VerticesPerChunk = 65000;

    public void CombineMeshes(List<GameObject> objects, Material material)
    {
        var chunks = new List<List<GameObject>>();
        var currentChunk = new List<GameObject>();
        int vertexCount = 0;

        foreach (var obj in objects)
        {
            var mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            if (vertexCount + mesh.vertexCount > VerticesPerChunk)
            {
                chunks.Add(currentChunk);
                currentChunk = new List<GameObject>();
                vertexCount = 0;
            }
            
            currentChunk.Add(obj);
            vertexCount += mesh.vertexCount;
        }
        
        if (currentChunk.Count > 0)
            chunks.Add(currentChunk);

        foreach (var chunk in chunks)
        {
            CombineChunk(chunk, material);
        }
    }

    private void CombineChunk(List<GameObject> objects, Material material)
    {
        var combines = new CombineInstance[objects.Count];
        
        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            combines[i].mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            combines[i].transform = obj.transform.localToWorldMatrix;
        }

        var go = new GameObject("CombinedMesh");
        go.transform.parent = transform;
        
        var mesh = new Mesh();
        mesh.CombineMeshes(combines);
        
        var mf = go.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        
        var mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterial = material;
        
        foreach (var obj in objects)
        {
            Destroy(obj);
        }
    }
} 