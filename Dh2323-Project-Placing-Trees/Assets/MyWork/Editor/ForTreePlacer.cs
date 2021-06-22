using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TreePlacer))]
    public class ForTreePlacer: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            TreePlacer treePlacer = (TreePlacer) target;
            if (DrawDefaultInspector())
            {
                if (treePlacer.autoUpdate)
                {
                    treePlacer.PlaceTrees();
                }
            }

            if (GUILayout.Button("Place Trees"))
            {
                treePlacer.PlaceTrees();
            }
            if (GUILayout.Button("Remove Old Trees"))
            {
                treePlacer.ResetPlacements();
            }

            base.OnInspectorGUI();
        }
    }
}