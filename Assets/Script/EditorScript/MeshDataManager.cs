using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshDataManager))]
public class MeshDataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshDataManager meshDataManager = (MeshDataManager)target;

        if (GUILayout.Button("Precompute Mesh Data"))
        {
            Mesh mesh = meshDataManager.GetComponent<MeshFilter>().sharedMesh;
            meshDataManager.PrecomputeMeshData(mesh);

            // Save data to the component
            EditorUtility.SetDirty(meshDataManager);
        }
    }
}
