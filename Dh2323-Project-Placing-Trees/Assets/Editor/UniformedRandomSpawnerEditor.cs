using UnityEditor;
using UnityEngine;
namespace Editor
{
    [CustomEditor(typeof(UniformedRandomSpawner))]
    public class UniformedRandomSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            UniformedRandomSpawner treePlacer = (UniformedRandomSpawner) target;
            
                if (GUILayout.Button("Run Script"))
                {
                    treePlacer.PlaceTrees();
                }
                base.OnInspectorGUI();
            
                if (GUILayout.Button("Remove Old Trees"))
                {
                    treePlacer.ResetPlacements();
                }
        }
        }
}
    
