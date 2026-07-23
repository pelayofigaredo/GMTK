using System.Collections;
using UnityEngine;

public class CharaterHandler : MonoBehaviour
{
    [SerializeField] GameObject[] characters;
    [SerializeField] ParticleSystem changeParticles;

    Animator activeAnimator;

    Animator[] animators;

    int current = 0;

    void Start()
    {
        animators = new Animator[characters.Length];
        for (int i = 0; i < characters.Length; i++)
        {
            animators[i] = characters[i].GetComponentInChildren<Animator>();
            characters[i].gameObject.SetActive(false);
        }
        characters[current].gameObject.SetActive(true);
        activeAnimator = animators[current];
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            int newIndex = (current + 1)%characters.Length;
            SetCharacter(newIndex);
        }
    }

    public void SetCharacter(int characterIndex)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeCharacterCR(characterIndex));
    }

    IEnumerator ChangeCharacterCR(int characterIndex)
    {
        changeParticles.Play();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        characters[current].gameObject.SetActive(false);

        current = characterIndex;
        characters[current].gameObject.SetActive(true);
        activeAnimator = animators[current];
    }
}
