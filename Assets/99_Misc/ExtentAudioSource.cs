using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtentAudioSource : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip[] _randomList;
    [SerializeField]
    private int _throttleFrame = 5;

    private HashSet<AudioClip> _playedClips = new();

    public void RandomPlay()
    {
        int roll = Random.Range(0, _randomList.Length);
        ThrottlePlayOneShot(_randomList[roll]);
    }

    public void ThrottlePlayOneShot(AudioClip clip)
    {
        if (_playedClips.Contains(clip))
        {
            return;
        }
        _audioSource.PlayOneShot(clip);
        _playedClips.Add(clip);
        Throttle_ResetPlayedClip().Forget();
    }
    private async UniTaskVoid Throttle_ResetPlayedClip()
    {
        await UniTask.DelayFrame(_throttleFrame);
        _playedClips.Clear();
    }
}
