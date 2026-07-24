using UnityEditor;

namespace IO_Scripts.Animation
{
    [CustomEditor(typeof(LightAnimator))]
    public class LightAnimatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Referencias a los campos
            SerializedProperty animationTime = serializedObject.FindProperty("animationTime");
            SerializedProperty launchEventOnEnd = serializedObject.FindProperty("launchEventOnEnd");

            SerializedProperty useIntensity = serializedObject.FindProperty("useIntensity");
            SerializedProperty minValue = serializedObject.FindProperty("minValue");
            SerializedProperty maxValue = serializedObject.FindProperty("maxValue");
            SerializedProperty intensityUsesAnimationCurve = serializedObject.FindProperty("intensityUsesAnimationCurve");
            SerializedProperty intensityAnimationCurve = serializedObject.FindProperty("intensityAnimationCurve");

            SerializedProperty useColor = serializedObject.FindProperty("useColor");
            SerializedProperty colorA = serializedObject.FindProperty("colorA");
            SerializedProperty colorB = serializedObject.FindProperty("colorB");
            SerializedProperty colorUsesAnimationCurve = serializedObject.FindProperty("colorUsesAnimationCurve");
            SerializedProperty colorAnimationCurve = serializedObject.FindProperty("colorAnimationCurve");

            SerializedProperty eventOnlyOnce = serializedObject.FindProperty("eventOnlyOnce");
            SerializedProperty OnAnimationCompleted = serializedObject.FindProperty("OnAnimationCompleted");

            EditorGUILayout.PropertyField(animationTime);
            EditorGUILayout.PropertyField(launchEventOnEnd);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Intensity", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useIntensity);
            if (useIntensity.boolValue)
            {
                EditorGUILayout.PropertyField(minValue);
                EditorGUILayout.PropertyField(maxValue);
                EditorGUILayout.PropertyField(intensityUsesAnimationCurve);
                if (intensityUsesAnimationCurve.boolValue)
                {
                    EditorGUILayout.PropertyField(intensityAnimationCurve);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Color", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useColor);
            if (useColor.boolValue)
            {
                EditorGUILayout.PropertyField(colorA);
                EditorGUILayout.PropertyField(colorB);
                EditorGUILayout.PropertyField(colorUsesAnimationCurve);
                if (colorUsesAnimationCurve.boolValue)
                {
                    EditorGUILayout.PropertyField(colorAnimationCurve);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(eventOnlyOnce);
            EditorGUILayout.PropertyField(OnAnimationCompleted);

            serializedObject.ApplyModifiedProperties();
        }
    }
}