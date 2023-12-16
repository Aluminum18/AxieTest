using AxieMixer.Unity;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacteStatsPanel : MonoBehaviour
{
    [SerializeField]
    private StringVariable _selectedCharacterId;
    [SerializeField]
    private SkeletonGraphic _characterUISkeleton;

    public void RefreshContent()
    {
        Mixer.SpawnSkeletonAnimation(_characterUISkeleton, _selectedCharacterId.Value, CharacterAnimator.GetCachedGene(_selectedCharacterId.Value));
    }
}
