using AxieMixer.Unity;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField]
    private string _axieId;

    private SkeletonAnimation _animator;
    private string _geneSearchUrl = "https://graphql-gateway.axieinfinity.com/graphql";

    // key = Id, value = gene
    private static Dictionary<string, string> _cachedGenes = new();

    public void ScaleX(float x)
    {
        _animator.skeleton.ScaleX = x;
    }

    private void Start()
    {
        _animator = GetComponent<SkeletonAnimation>();
        Mixer.Init();
        Async_SetUpAnimator().Forget();
    }

    private async UniTaskVoid Async_SetUpAnimator()
    {
        string gene = await GetGene(_axieId);
        Mixer.SpawnSkeletonAnimation(_animator, _axieId, gene);

        await UniTask.WaitUntil(() => _animator.SkeletonDataAsset != null);
        _animator.state.SetAnimation(0, "action/idle/normal", true);
    }

    private async UniTask<string> GetGene(string axieId)
    {
        _cachedGenes.TryGetValue(axieId, out var genes);
        if (!string.IsNullOrEmpty(genes))
        {
            return genes;
        }

        JObject jPayload = new()
        {
            new JProperty("query", "{ axie (axieId: \"" + axieId + "\") { id, genes, newGenes}}")
        };

        byte[] uploadData = new System.Text.UTF8Encoding().GetBytes(jPayload.ToString().ToCharArray());
        var result = await RequestSenderAsync.SendPostRequest(_geneSearchUrl, uploadData);
        if (result.responseCode != 200)
        {
            return "0x2000000000000300008100e08308000000010010088081040001000010a043020000009008004106000100100860c40200010000084081060001001410a04406";
        }
        var jObjectResult = JObject.Parse(result.text);
        return jObjectResult["data"]["axie"]["newGenes"].ToString();
    }
}
