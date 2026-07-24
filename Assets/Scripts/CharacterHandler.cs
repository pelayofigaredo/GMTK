using System;
using System.Collections;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    [SerializeField] GameObject[] characters;
    [SerializeField] ParticleSystem changeParticles;
    [SerializeField] float sideStrafeRotation = 90;

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

    internal void SetParameters(Transform playerTransform, Vector3 desiredDirection)
    {
        float speed = (desiredDirection.magnitude > 0.01f)?1:0;

        if(speed <= 0)
        {
            activeAnimator.SetFloat("Speed", 0);
            activeAnimator.SetFloat("ZDirection", 0);
            activeAnimator.SetFloat("XDirection", 0);

            transform.localRotation = Quaternion.identity;
        }
        else
        {
            Vector3 direction = playerTransform.InverseTransformDirection(desiredDirection);

            activeAnimator.SetFloat("Speed", speed);
            activeAnimator.SetFloat("ZDirection", direction.z);
            activeAnimator.SetFloat("XDirection", direction.x);

            transform.localRotation = Quaternion.Euler(0,direction.x *sideStrafeRotation,0);
        }

    }

    internal void Attack(IAttacker attacker)
    {
        activeAnimator.SetTrigger("attack");
    }
}
