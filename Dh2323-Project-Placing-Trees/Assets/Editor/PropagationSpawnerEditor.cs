using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PropagationSpawner))]
    public class PropagationSpawnerEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            PropagationSpawner treePlacer = (PropagationSpawner) target;
            
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