using AxieMixer.Unity;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspecCharacterAnimator : MonoBehaviour
{
    [SerializeField]
    private StringVariable _inspecCharacterId;
    [SerializeField]
    private SkeletonAnimation _animator;

    private string _currentCharacterId;

    private void UpdateAnimator(string characterId)
    {
        if (characterId == _currentCharacterId)
        {
            return;
        }

        Mixer.SpawnSkeletonAnimation(_animator, characterId, CharacterAnimator.GetCachedGene(characterId));
        _animator.state.SetAnimation(0, "action/idle/normal", true);
    }

    private void OnEnable()
    {
        _inspecCharacterId.OnValueChange += UpdateAnimator;
    }

    private void Update()
    {
        _animator.Update(Time.unscaledDeltaTime);
        _animator.LateUpdate();
    }

    private void OnDisable()
    {
        _inspecCharacterId.OnValueChange -= UpdateAnimator;
    }
}
