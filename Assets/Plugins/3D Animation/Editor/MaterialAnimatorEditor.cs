using UnityEngine;
using UnityEditor;

namespace IO_Scripts.MaterialAnimation
{
    #region Material Animator
    [CustomEditor(typeof(MaterialAnimator))]
    public class MaterialAnimatorEditor : Editor
    {
        SerializedProperty animationTargets;
        SerializedProperty changes;
        SerializedProperty time;
        SerializedProperty useCurve;
        SerializedProperty curve;
        SerializedProperty initialValue;

        private void OnEnable()
        {
            animationTargets = serializedObject.FindProperty("targets");
            changes = serializedObject.FindProperty("changes");

            time = serializedObject.FindProperty("time");
            useCurve = serializedObject.FindProperty("useCurve");
            curve = serializedObject.FindProperty("curve");
            initialValue = serializedObject.FindProperty("initialValue");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MaterialAnimator animator = (MaterialAnimator)target;

            // Estilo para los encabezados
            GUIStyle headerStyle = new GUIStyle();
            headerStyle.fontSize = 20;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.normal.textColor = Color.white;

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Play"))
                {
                    animator.Play();
                }
                GUILayout.Space(5);
                if (GUILayout.Button("Reverse"))
                {
                    animator.Reverse();
                }
                GUILayout.Space(5);
                if (GUILayout.Button("Stop"))
                {
                    animator.Stop();
                }
                GUILayout.Space(20);


            }

            float newTime = EditorGUILayout.Slider("Current Value", animator.T, 0, 1);
            if (newTime != animator.T && Application.isPlaying)
            {
                animator.SetTSafe(newTime);
            }

            //Animation Configuration
            GUILayout.Label("Animation Configuration", headerStyle);
            GUILayout.Space(10);

            EditorGUILayout.PropertyField(time, new GUIContent("Time"));
            EditorGUILayout.PropertyField(useCurve, new GUIContent("Use Curve"));
            if (useCurve.boolValue)
            {
                EditorGUILayout.PropertyField(curve, new GUIContent("Curve"));
            }
            EditorGUILayout.Slider(initialValue, 0f, 1f, new GUIContent("Initial Value"));

            GUILayout.Space(20);

            // Render Targets
            GUILayout.Label("Render Targets", headerStyle);
            GUILayout.Space(10);

            // Show warning if Targets is empty
            if (animationTargets.arraySize == 0)
            {
                EditorGUILayout.HelpBox("La lista de Render Targets está vacía.", MessageType.Warning);
            }

            // l_Targets
            for (int i = 0; i < animationTargets.arraySize; i++)
            {
                SerializedProperty target = animationTargets.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical("box");
                GUILayout.Space(5);

                EditorGUILayout.PropertyField(target.FindPropertyRelative("renderer"), new GUIContent("Renderer"));
                if (target.FindPropertyRelative("renderer").objectReferenceValue != null)
                {
                    EditorGUILayout.PropertyField(target.FindPropertyRelative("index"), new GUIContent("Index"));
                }

                if (GUILayout.Button("Remove Target", GUILayout.Width(120)))
                {
                    animationTargets.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(10);
            }

            if (GUILayout.Button("Add Target"))
            {
                animationTargets.InsertArrayElementAtIndex(animationTargets.arraySize);
            }

            GUILayout.Space(30);

            // Sección Property Targets
            GUILayout.Label("Property Targets", headerStyle);
            GUILayout.Space(10);

            // Mostrar advertencia si l_Changes está vacío
            if (changes.arraySize == 0)
            {
                EditorGUILayout.HelpBox("La lista de Property Targets está vacía.", MessageType.Warning);
            }

            // l_Changes
            if (changes != null)
            {
                for (int i = 0; i < changes.arraySize; i++)
                {
                    SerializedProperty change = changes.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(change, true);

                    if (GUILayout.Button("Remove Change", GUILayout.Width(120)))
                    {
                        changes.DeleteArrayElementAtIndex(i);
                    }
                }
            }

            if (GUILayout.Button("Add Change"))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Color"), false, () => AddChange<MaterialChangeColor>());
                menu.AddItem(new GUIContent("Float"), false, () => AddChange<MaterialChangeFloat>());
                menu.AddItem(new GUIContent("Int"), false, () => AddChange<MaterialChangeInt>());
                menu.AddItem(new GUIContent("Texture"), false, () => AddChange<MaterialChangeTexture>());
                menu.AddItem(new GUIContent("Vector2"), false, () => AddChange<MaterialChangeVector2>());
                menu.AddItem(new GUIContent("Vector3"), false, () => AddChange<MaterialChangeVector3>());
                menu.ShowAsContext();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AddChange<T>() where T : MaterialChange, new()
        {
            MaterialAnimator animator = (MaterialAnimator)target;
            animator.AddNewChange<T>();
            EditorUtility.SetDirty(animator);
            serializedObject.Update();
        }
    }
    #endregion

    //Change classes
    #region MaterialChange
    [CustomPropertyDrawer(typeof(MaterialChange))]
    public class MaterialChangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            GUIContent titleLabel = new GUIContent(GetPropertyName() + property.FindPropertyRelative("Key").stringValue);

            // Calcular alturas y espacios
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            // Rect para la etiqueta del título
            Rect titleRect = new Rect(position.x, position.y, position.width, lineHeight);

            // Dibujar la etiqueta del título
            EditorGUI.LabelField(titleRect, titleLabel, EditorStyles.boldLabel);

            // Ajustar la posición para los siguientes campos
            position.y += lineHeight + verticalSpacing;

            // Rectángulos para cada campo
            Rect keyRect = new Rect(position.x, position.y, position.width, lineHeight);
            Rect initialValueRect = new Rect(position.x, position.y + lineHeight + verticalSpacing, position.width, lineHeight);
            Rect endValueRect = new Rect(position.x, position.y + 2 * (lineHeight + verticalSpacing), position.width, lineHeight);

            // Dibujar los campos
            EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("Key"), new GUIContent("Key"));
            EditorGUI.PropertyField(initialValueRect, property.FindPropertyRelative("InitialValue"), new GUIContent("Valor Inicial"));
            EditorGUI.PropertyField(endValueRect, property.FindPropertyRelative("EndValue"), new GUIContent("Valor Final"));

            EditorGUI.EndProperty();
        }

        protected virtual string GetPropertyName()
        {
            return "";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 4; // Key, InitialValue, EndValue
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            return fieldCount * lineHeight + (fieldCount - 1) * verticalSpacing;
        }
    }

    #endregion

    #region Change Float

    [CustomPropertyDrawer(typeof(MaterialChangeFloat))]
    public class MaterialChangeFloatDrawer : MaterialChangeDrawer
    {
        protected override string GetPropertyName()
        {
            return "Float property: ";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 4; // Key, InitialValue, EndValue
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            return fieldCount * lineHeight + (fieldCount - 1) * verticalSpacing;
        }
    }
    #endregion

    #region Change Int

    [CustomPropertyDrawer(typeof(MaterialChangeInt))]
    public class MaterialChangeIntDrawer : MaterialChangeDrawer
    {
        protected override string GetPropertyName()
        {
            return "Int property: ";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 4; // Título, Key, InitialValue, EndValue
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            return fieldCount * lineHeight + (fieldCount - 1) * verticalSpacing;
        }
    }
    #endregion

    #region Change Color
    [CustomPropertyDrawer(typeof(MaterialChangeColor))]
    public class MaterialChangeColorDrawer : MaterialChangeDrawer
    {
        protected override string GetPropertyName()
        {
            return "Color property: ";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 4; // Título, Key, InitialValue, EndValue
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            return fieldCount * lineHeight + (fieldCount - 1) * verticalSpacing;
        }
    }
    #endregion

    #region Change Texture

    [CustomPropertyDrawer(typeof(MaterialChangeTexture))]
    public class MaterialChangeTextureDrawer : MaterialChangeDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            GUIContent titleLabel = new GUIContent("Texture property: " + property.FindPropertyRelative("Key").stringValue);

            // Calcular alturas y espacios
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            // Rect para la etiqueta del título
            Rect titleRect = new Rect(position.x, position.y, position.width, lineHeight);

            // Dibujar la etiqueta del título
            EditorGUI.LabelField(titleRect, titleLabel, EditorStyles.boldLabel);

            // Ajustar la posición para los siguientes campos
            position.y += lineHeight + verticalSpacing;

            // Rectángulos para cada campo
            Rect keyRect = new Rect(position.x, position.y, position.width, lineHeight);
            Rect initialValueRect = new Rect(position.x, position.y + lineHeight + verticalSpacing, position.width, lineHeight);
            Rect endValueRect = new Rect(position.x, position.y + 2 * (lineHeight + verticalSpacing), position.width, lineHeight);
            Rect thresholdRect = new Rect(position.x, position.y + 3 * (lineHeight + verticalSpacing), position.width, lineHeight);

            // Dibujar los campos
            EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("Key"), new GUIContent("Key"));
            EditorGUI.PropertyField(initialValueRect, property.FindPropertyRelative("InitialValue"), new GUIContent("Valor Inicial"));
            EditorGUI.PropertyField(endValueRect, property.FindPropertyRelative("EndValue"), new GUIContent("Valor Final"));
            EditorGUI.PropertyField(thresholdRect, property.FindPropertyRelative("ChangeThreshold"), new GUIContent("Umbral de Cambio"));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 5; // Título, Key, InitialValue, EndValue, changeThreshold
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            return fieldCount * lineHeight + (fieldCount - 1) * verticalSpacing;
        }
    }
    #endregion

    #region Change Vector2
    [CustomPropertyDrawer(typeof(MaterialChangeVector2))]
    public class MaterialChangeVector2Drawer : MaterialChangeDrawer
    {
        protected override string GetPropertyName()
        {
            return "Vector2 property: ";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 4; // Título, Key, InitialValue, EndValue
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            return fieldCount * lineHeight + (fieldCount - 1) * verticalSpacing;
        }
    }
    #endregion

    #region Change Vector3

    [CustomPropertyDrawer(typeof(MaterialChangeVector3))]
    public class MaterialChangeVector3Drawer : MaterialChangeDrawer
    {
        protected override string GetPropertyName()
        {
            return "Vector3 property: ";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 4; // Título, Key, InitialValue, EndValue
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            return fieldCount * lineHeight + (fieldCount - 1) * verticalSpacing;
        }
    }
    #endregion
}
