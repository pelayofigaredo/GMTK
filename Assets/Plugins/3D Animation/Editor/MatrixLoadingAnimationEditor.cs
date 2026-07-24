using UnityEngine;
using UnityEditor;
namespace IO_Scripts.Animation
{
    [CustomEditor(typeof(MatrixLoadingAnimation))]
    public class MatrixLoadingAnimationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MatrixLoadingAnimation animation = (MatrixLoadingAnimation)target;
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate copies of renderes for the animation"))
            {
                animation.GenerateFake();
                animation.UseGhostObjects = true;
            }
        }
    }
}