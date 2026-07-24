using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IO_Scripts.Animation
{
    public class MatrixLoadingAnimation : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] float distance = 100;
        [SerializeField] float waitBetweenPhases = 2;
        [SerializeField] bool autoPlayeNextPhase = true;
        [SerializeField] bool useGhostObjects = false;
        [SerializeField] bool hideRenderers = false;
        [SerializeField] Phase[] l_Phases;
        [SerializeField] UnityEvent OnAnimationEnded;

        bool isPlaying = false;
        bool hasFinished = false;

        [SerializeField] int currentPhaseIndex = 0;

        public bool UseGhostObjects { get => useGhostObjects; set => useGhostObjects = value; }

        void Start()
        {
            if (!useGhostObjects)
            {
                foreach (Phase p in l_Phases)
                {
                    p.PrepareElements(p.l_OrignalTransforms, distance);
                }
            }
            else
            {
                foreach (Phase p in l_Phases)
                {
                    p.ToggleOriginals(false);
                }
            }
        }

        IEnumerator AnimationCR()
        {
            while (isPlaying)
            {
                bool phaseEnded = l_Phases[currentPhaseIndex].UpdatePhase(Time.deltaTime);
                if (phaseEnded)
                {
                    if (currentPhaseIndex == l_Phases.Length - 1)
                    {
                        isPlaying = false;
                        AnimationEnded();
                    }
                    else if (autoPlayeNextPhase)
                    {
                        currentPhaseIndex++;
                        StartCoroutine(PhaseWaitCR());
                    }
                    else
                    {
                        currentPhaseIndex++;
                        isPlaying = false;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (!hasFinished && !isPlaying)
            {
                isPlaying = true;
                StartCoroutine(AnimationCR());
            }
        }

        [ContextMenu("Restart")]
        public void Restart()
        {
            foreach (Phase p in l_Phases)
            {
                p.Reset();
                if (useGhostObjects)
                {
                    p.ToggleOriginals(false);
                    p.ToggleAnimatedGameObjects(true);
                }
            }
            currentPhaseIndex = 0;
            Play();
        }

        void AnimationEnded()
        {
            hasFinished = true;
            OnAnimationEnded?.Invoke();
        }

        IEnumerator PhaseWaitCR()
        {
            isPlaying = false;
            yield return new WaitForSeconds(waitBetweenPhases);
            isPlaying = true;
        }

#if UNITY_EDITOR
        [ContextMenu("Generate fake versions for animation")]
        public void GenerateFake()
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            foreach (Phase p in l_Phases)
            {
                p.PrepareElements(p.l_OrignalTransforms, distance, false);
                for (int i = 0; i < p.l_OrignalTransforms.Length; i++)
                {
                    GameObject ghostGameObject = CloneRenderers(p.l_OrignalTransforms[i]);
                    ghostGameObject.transform.SetParent(transform, true);
                    p.L_Elements[i].OriginalGameObject = p.l_OrignalTransforms[i].gameObject;
                    ghostGameObject.transform.position = p.L_Elements[i].Origin;
                    p.L_Elements[i].Transfor = ghostGameObject.transform;
                }
            }
        }

        public GameObject CloneRenderers(Transform original)
        {
            Renderer[] originalRenderers = original.gameObject.GetComponents<Renderer>();
            MeshFilter[] originalFilters = original.gameObject.GetComponents<MeshFilter>();
            GameObject newGameObject = new GameObject(original.name + " animation clone");

            for (int i = 0; i < originalFilters.Length; ++i)
            {
                UnityEditorInternal.ComponentUtility.CopyComponent(originalFilters[i]);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newGameObject);
            }

            for (int i = 0; i < originalRenderers.Length; ++i)
            {
                UnityEditorInternal.ComponentUtility.CopyComponent(originalRenderers[i]);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newGameObject);
            }

            newGameObject.transform.position = original.position;
            newGameObject.transform.rotation = original.rotation;
            newGameObject.transform.localScale = original.localScale;

            //Manage children
            if (original.childCount > 0)
            {
                List<Transform> children = new List<Transform>();
                for (int i = 0; i < original.childCount; ++i)
                {
                    children.Add(CloneRenderers(original.GetChild(i)).transform);
                }
                foreach (Transform child in children)
                {
                    child.transform.SetParent(newGameObject.transform, true);
                }
            }

            return newGameObject;
        }

#endif

        [System.Serializable]
        class Phase
        {
            public float ElementAnimationTime;
            public float ElementAnimationTimeVariance;
            public AnimationCurve AnimCurve;
            public Directions IncomingDirections;
            public Transform[] l_OrignalTransforms;
            public Element[] L_Elements;
            public List<Element> L_ActiveElements;


            [SerializeField] float currentTime;
            [SerializeField] float totalTime;


            public bool UpdatePhase(float deltaTime)
            {
                currentTime += deltaTime;
                for (int i = L_ActiveElements.Count - 1; i >= 0; --i)
                {
                    bool finished = L_ActiveElements[i].UpdateElement(deltaTime, AnimCurve);
                    if (finished)
                    {
                        if (L_ActiveElements[i].OriginalGameObject != null)
                        {
                            L_ActiveElements[i].OriginalGameObject.SetActive(true);
                            L_ActiveElements[i].Transfor.gameObject.SetActive(false);
                        }
                        L_ActiveElements.RemoveAt(i);

                    }
                }

                if (currentTime >= totalTime)
                    return true;

                return false;
            }

            public void PrepareElements(Transform[] transforms, float distance, bool setPositionsToOrigin = true)
            {
                L_ActiveElements = new List<Element>(transforms.Length);
                for (int i = 0; i < transforms.Length; ++i)
                {
                    float elementTime = ElementAnimationTime + UnityEngine.Random.Range(-ElementAnimationTimeVariance, ElementAnimationTimeVariance);
                    if (elementTime > totalTime)
                        totalTime = elementTime;
                    L_ActiveElements.Add(new Element(
                                                transforms[i],
                                                (transforms[i].position + (GetIncomingDirection() * distance)),
                                                transforms[i].position,
                                                1 / elementTime,
                                                setPositionsToOrigin
                                              )
                                    );
                }
                L_Elements = L_ActiveElements.ToArray();
            }

            public void Reset()
            {
                L_ActiveElements = new List<Element>(L_Elements);
                currentTime = 0;
                foreach (Element e in L_ActiveElements)
                {
                    e.Reset();
                }
            }

            public void ToggleOriginals(bool toggle)
            {
                foreach (Element e in L_Elements)
                {
                    if (e.OriginalGameObject != null)
                    {
                        e.OriginalGameObject.SetActive(toggle);
                    }
                }
            }

            public void ToggleAnimatedGameObjects(bool toggle)
            {
                foreach (Element e in L_Elements)
                {
                    if (e.Transfor != null)
                    {
                        e.Transfor.gameObject.SetActive(toggle);
                    }
                }
            }

            Vector3 GetIncomingDirection()
            {
                Vector3 direction = Vector3.zero;
                switch (IncomingDirections)
                {
                    case Directions.All:
                        int d = UnityEngine.Random.Range(0, 6);
                        switch (d)
                        {
                            case 0:
                                return Vector3.up;
                            case 1:
                                return Vector3.down;
                            case 2:
                                return Vector3.left;
                            case 3:
                                return Vector3.right;
                            case 4:
                                return Vector3.forward;
                        }
                        return Vector3.back;
                    case Directions.TopAndDown:
                        if (UnityEngine.Random.Range(0, 2) == 0)
                            return Vector3.up;
                        else
                            return Vector3.down;
                    case Directions.FrontAndBack:
                        if (UnityEngine.Random.Range(0, 2) == 0)
                            return Vector3.forward;
                        else
                            return Vector3.back;
                    case Directions.RightAndLeft:
                        if (UnityEngine.Random.Range(0, 2) == 0)
                            return Vector3.right;
                        else
                            return Vector3.left;
                    case Directions.Sides:
                        int s = UnityEngine.Random.Range(0, 4);
                        switch (s)
                        {
                            case 0:
                                return Vector3.left;
                            case 1:
                                return Vector3.right;
                            case 2:
                                return Vector3.forward;
                        }
                        return Vector3.back;
                    case Directions.SidesAndTop:
                        int sT = UnityEngine.Random.Range(0, 5);
                        switch (sT)
                        {
                            case 0:
                                return Vector3.left;
                            case 1:
                                return Vector3.right;
                            case 2:
                                return Vector3.forward;
                            case 3:
                                return Vector3.back;
                        }
                        return Vector3.up;
                    case Directions.Top:
                        return Vector3.up;
                    case Directions.Down:
                        return Vector3.down;
                    case Directions.Left:
                        return Vector3.left;
                    case Directions.Right:
                        return Vector3.right;
                    case Directions.Forward:
                        return Vector3.forward;
                    case Directions.Backwards:
                        return Vector3.back;
                }
                return direction;
            }
        }

        [System.Serializable]
        enum Directions
        {
            All,
            TopAndDown,
            FrontAndBack,
            RightAndLeft,
            Sides,
            SidesAndTop,
            Top,
            Down,
            Left,
            Right,
            Forward,
            Backwards
        }

        [System.Serializable]
        class Element
        {
            public Transform Transfor;
            public GameObject OriginalGameObject;
            public Vector3 Origin;
            public Vector3 Destination;
            public float TIncrease;
            float t;

            public Element(Transform transform, Vector3 origin, Vector3 destination, float tIncrease, bool setPosition = true)
            {
                this.Transfor = transform;
                this.Origin = origin;
                this.Destination = destination;
                this.TIncrease = tIncrease;
                t = 0;
                if (setPosition)
                    transform.position = origin;
            }



            public bool UpdateElement(float deltaTime, AnimationCurve animationCurve)
            {
                t += TIncrease * deltaTime;
                Transfor.position = Vector3.Lerp(Origin, Destination, animationCurve.Evaluate(t));

                if (t >= 1)
                    return true;
                return false;
            }

            public void Reset()
            {
                t = 0;
                Transfor.position = Origin;
            }
        }
    }
}