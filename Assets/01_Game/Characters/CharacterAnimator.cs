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
    [SerializeField]
    private List<string> _animationNames;

    private SkeletonAnimation _animator;

    private string _geneSearchUrl = "https://graphql-gateway.axieinfinity.com/graphql";

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
    }

    private async UniTask<string> GetGene(string axieId)
    {
        JObject jPayload = new()
        {
            new JProperty("query", "{ axie (axieId: \"" + axieId + "\") { id, genes, newGenes}}")
        };

        byte[] uploadData = new System.Text.UTF8Encoding().GetBytes(jPayload.ToString().ToCharArray());
        var result = await RequestSenderAsync.SendPostRequest(_geneSearchUrl, uploadData);
        if (result.responseCode != 200)
        {
            return "";
        }
        var jObjectResult = JObject.Parse(result.text);
        return jObjectResult["data"]["axie"]["newGenes"].ToString();
    }
}
