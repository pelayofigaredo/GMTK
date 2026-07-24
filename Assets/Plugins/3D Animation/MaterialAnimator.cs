using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IO_Scripts.MaterialAnimation
{
    public class MaterialAnimator : MonoBehaviour
    {
        public RendererIndexPair[] targets;
        [SerializeField, SerializeReference]
        public List<MaterialChange> changes = new List<MaterialChange>();

        [SerializeField] float time = 1;
        [SerializeField] bool useCurve;
        [SerializeField] AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        [Range(0, 1)] float initialValue = 0;

        float step;
        float t = 0;

        bool active = false;
        bool goingToDestination = true;

        Coroutine animationCoroutine;

        Material[] l_Materials;

        public List<MaterialChange> Changes { get => changes; }
        public float T { get => t; }

        void Awake()
        {
            l_Materials = new Material[targets.Length];
            List<Material> aux = new List<Material>();

            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].renderer.GetMaterials(aux);
                l_Materials[i] = aux[targets[i].index];
            }

            t = initialValue;
            foreach (MaterialChange change in Changes)
            {
                change.SetT(t, l_Materials);
            }
        }

        IEnumerator AnimationCR()
        {
            while (active)
            {
                t += (goingToDestination) ? step * Time.deltaTime : step * Time.deltaTime * -1;
                float value = Mathf.Clamp01(t);
                if (useCurve)
                    Mathf.Clamp01(value = curve.Evaluate(value));

                if (t >= 1)
                {
                    active = false;
                    t = 1;
                }
                else if (t <= 0)
                {
                    active = false;
                    t = 0;
                }

                SetT(value);
                yield return new WaitForEndOfFrame();
            }
            animationCoroutine = null;
        }

        private void LaunchCoroutine()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            active = true;
            animationCoroutine = StartCoroutine(AnimationCR());
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (Changes != null && Changes.Count > 0)
            {
                if (time > 0)
                {
                    goingToDestination = true;
                    step = 1 / time;
                    LaunchCoroutine();
                }
                else
                {
                    SetT(1);
                }
            }
        }

        [ContextMenu("Play reverse")]
        public void Reverse()
        {
            if (Changes != null && Changes.Count > 0)
            {
                if (time > 0)
                {
                    goingToDestination = false;
                    step = 1 / time;
                    LaunchCoroutine();
                }
                else
                {
                    SetT(0);
                }
            }
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            active = false;
        }

        public void SetT(float t)
        {
            this.t = t;
            foreach (MaterialChange c in Changes)
            {
                c.SetT(t, l_Materials);
            }
        }

        public void SetTSafe(float t)
        {
            if (l_Materials == null || l_Materials.Length == 0)
            {
                l_Materials = new Material[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    l_Materials[i] = targets[i].renderer.materials[targets[i].index];
                }
            }

            this.t = t;
            foreach (MaterialChange c in Changes)
            {
                c.SetT(t, l_Materials);
            }
        }

        public void AddNewChange<T>() where T : MaterialChange, new()
        {
            T newChange = new T();
            Changes.Add(newChange);
        }

        public void RemoveChange(MaterialChange change)
        {
            Changes.Remove(change);
        }

        public void SetTargets(Renderer[] renderers)
        {
            int[] indices = new int[renderers.Length];
            SetTargets(renderers, indices);
        }

        public void SetTargets(Renderer[] renderers, int[] indices)
        {
            RendererIndexPair[] newTargets = new RendererIndexPair[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                newTargets[i] = new RendererIndexPair
                {
                    renderer = renderers[i],
                    index = indices[i]
                };
            }

            targets = newTargets;
        }

        public void SetPropertyValues(int v, Vector2 vector2)
        {
            if (v >= 0 && v < Changes.Count)
            {
                MaterialChange change = Changes[v];
                switch (change)
                {
                    case MaterialChangeFloat floatChange:
                        floatChange.InitialValue = vector2.x;
                        floatChange.EndValue = vector2.y;
                        break;
                    case MaterialChangeInt intChange:
                        intChange.InitialValue = (int)vector2.x;
                        intChange.EndValue = (int)vector2.y;
                        break;
                    case MaterialChangeColor colorChange:
                        // Not applicable
                        break;
                    case MaterialChangeTexture textureChange:
                        // Not applicable
                        break;
                    case MaterialChangeVector2 vector2Change:
                        vector2Change.InitialValue = new Vector2(vector2.x, vector2.x);
                        vector2Change.EndValue = new Vector2(vector2.y, vector2.y);
                        break;
                    case MaterialChangeVector3 vector3Change:
                        vector3Change.InitialValue = new Vector3(vector2.x, vector2.x, vector2.x);
                        vector3Change.EndValue = new Vector3(vector2.y, vector2.y, vector2.y);
                        break;
                }
            }
        }

        //--------------------- AUX CLASES ---------------------//

        [System.Serializable]
        public class RendererIndexPair
        {
            public Renderer renderer;
            public int index;
        }


    }

    //Change classes
    #region MaterialChange
    [System.Serializable]
    public abstract class MaterialChange
    {
        public string Key;
        public abstract void SetT(float t, Material[] targets);
    }
    #endregion

    #region Change Float
    [System.Serializable]
    public class MaterialChangeFloat : MaterialChange
    {
        public float InitialValue = 0;
        public float EndValue = 1;
        public override void SetT(float t, Material[] targets)
        {
            float currentValue = Mathf.Lerp(InitialValue, EndValue, t);
            foreach (Material m in targets)
            {
                m.SetFloat(Key, currentValue);
            }
        }
    }
    #endregion

    #region Change Int

    [System.Serializable]
    public class MaterialChangeInt : MaterialChange
    {
        public int InitialValue = 0;
        public int EndValue = 1;

        public override void SetT(float t, Material[] targets)
        {
            int currentValue = (int)Mathf.Lerp(InitialValue, EndValue, t);
            foreach (Material m in targets)
            {
                m.SetInt(Key, currentValue);
            }
        }
    }
    #endregion

    #region Change Color

    [System.Serializable]
    public class MaterialChangeColor : MaterialChange
    {
        public Color InitialValue = Color.black;
        public Color EndValue = Color.white;

        public override void SetT(float t, Material[] targets)
        {
            Color currentValue = Color.Lerp(InitialValue, EndValue, t);
            foreach (Material m in targets)
            {
                m.SetColor(Key, currentValue);
            }
        }
    }
    #endregion

    #region Change Texture

    [System.Serializable]
    public class MaterialChangeTexture : MaterialChange
    {
        public Texture InitialValue;
        public Texture EndValue;
        public float ChangeThreshold = 0.5f;

        public override void SetT(float t, Material[] targets)
        {
            Texture currentTexture = (t > ChangeThreshold) ? EndValue : InitialValue;
            foreach (Material m in targets)
            {
                m.SetTexture(Key, currentTexture);
            }
        }
    }
    #endregion

    #region Change Vector2

    [System.Serializable]
    public class MaterialChangeVector2 : MaterialChange
    {
        public Vector2 InitialValue;
        public Vector2 EndValue;

        public override void SetT(float t, Material[] targets)
        {
            Vector2 value = Vector2.Lerp(InitialValue, EndValue, t);
            Vector4 current = new Vector4(value.x, value.y, 0, 0);
            foreach (Material m in targets)
            {
                m.SetVector(Key, current);
            }
        }
    }
    #endregion

    #region Change Vector3

    [System.Serializable]
    public class MaterialChangeVector3 : MaterialChange
    {
        public Vector3 InitialValue;
        public Vector3 EndValue;

        public override void SetT(float t, Material[] targets)
        {
            Vector3 value = Vector3.Lerp(InitialValue, EndValue, t);
            Vector4 current = new Vector4(value.x, value.y, value.z, 0);
            foreach (Material m in targets)
            {
                m.SetVector(Key, current);
            }
        }
    }
    #endregion
}


