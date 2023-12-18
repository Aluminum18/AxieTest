using AxieMixer.Unity;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CharacterAnimator : MonoBehaviour
{
    private static bool _initiated = false;
    // key = Id, value = gene
    private static Dictionary<string, string> _cachedGenes = new();
    // key = Id, value = animation names
    private static Dictionary<string, List<string>> _attackAnimationDict = new();
    private static Dictionary<string, List<string>> _idleAnimationDict = new();

    [SerializeField]
    private string _axieId;
    public string AxieId => _axieId;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onACharacterFinishedSetUp;

    private SkeletonAnimation _animator;
    private const string _geneSearchUrl = "https://graphql-gateway.axieinfinity.com/graphql";
    private const string RETRIEVING_GENE = "retrieving";

    public static string GetCachedGene(string _axieId)
    {
        _cachedGenes.TryGetValue(_axieId, out var cachedGenes);
        return cachedGenes;
    }

    public void ScaleX(float x)
    {
        _animator.skeleton.ScaleX = x;
    }

    /// <summary>
    /// -1: character face to the left, 1: charater face to the right
    /// </summary>
    public float GetFaceDirection()
    {
        return -Mathf.Sign(_animator.skeleton.ScaleX);      
    }

    public float DoAttackAnimation()
    {
        _attackAnimationDict.TryGetValue(_axieId, out var attacks);

        int roll = Random.Range(0, attacks.Count);
        _animator.state.SetAnimation(0, attacks[roll], false);
        return _animator.state.GetCurrent(0).Animation.Duration;
    }

    public void DoIdleAnimation()
    {
        _idleAnimationDict.TryGetValue(_axieId, out var idles);
        int roll = Random.Range(0, idles.Count);
        _animator.state.SetAnimation(0, idles[roll], true);
    }

    public void DoMoveAnimation()
    {
        _animator.state.SetAnimation(0, "action/move-forward", false);
    }

    public float DoDefeatedAnimation()
    {
        _animator.state.SetAnimation(0, "defense/hit-by-normal-dramatic", false);
        return _animator.state.GetCurrent(0).Animation.Duration;
    }

    private void Start()
    {
        _animator = GetComponent<SkeletonAnimation>();
        if (!_initiated)
        {
            Mixer.Init();
            _initiated = true;
        }
        Async_SetUpAnimator().Forget();
    }

    private async UniTaskVoid Async_SetUpAnimator()
    {
        string gene = await GetGene(_axieId);
        if (gene == RETRIEVING_GENE)
        {
            await UniTask.WaitUntil(() => _cachedGenes[_axieId] != RETRIEVING_GENE);
        }

        gene = _cachedGenes[_axieId];
        Mixer.SpawnSkeletonAnimation(_animator, _axieId, gene);

        await UniTask.NextFrame(); // SpawnSkeletonAnimtion handles very heavy logic, executing animation in next frame for smoother animation
        _onACharacterFinishedSetUp.Raise();
        _idleAnimationDict.TryGetValue(_axieId, out var idles);
        if (idles != null)
        {
            DoIdleAnimation();
            return;
        }
        CacheAnimationNames();
    }

    private void CacheAnimationNames()
    {
        List<string> animations = Mixer.Builder.axieMixerMaterials.GetMixerStuff(AxieFormType.Normal).GetAnimatioNames();

        List<string> idles = new();
        _idleAnimationDict.Add(_axieId, idles);
        List<string> attacks = new();
        _attackAnimationDict.Add(_axieId, attacks);

        for (int i = 0; i < animations.Count; i++)
        {
            if (animations[i].Contains("action/idle"))
            {
                idles.Add(animations[i]);
            }

            if (animations[i].Contains("attack/melee"))
            {
                attacks.Add(animations[i]);
            }
        }
    }

    private async UniTask<string> GetGene(string axieId)
    {
        _cachedGenes.TryGetValue(axieId, out var genes);
        if (!string.IsNullOrEmpty(genes))
        {
            return genes;
        }
        _cachedGenes[axieId] = RETRIEVING_GENE;

        JObject jPayload = new()
        {
            new JProperty("query", "{ axie (axieId: \"" + axieId + "\") { id, genes, newGenes}}")
        };

        byte[] uploadData = new System.Text.UTF8Encoding().GetBytes(jPayload.ToString());
        var result = await RequestSenderAsync.SendPostRequest(_geneSearchUrl, uploadData);

        string retrievedGenes = "0x2000000000000300008100e08308000000010010088081040001000010a043020000009008004106000100100860c40200010000084081060001001410a04406";
        if (result.responseCode != 200)
        {
            Debug.LogError("Failed to fetch genes info, use static genes instead!");
            return retrievedGenes;
        }

        var jObjectResult = JObject.Parse(result.text);
        retrievedGenes = jObjectResult["data"]["axie"]["newGenes"].ToString();
        _cachedGenes[_axieId] = retrievedGenes;
        return genes;
    }
}
