using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private float _fadeDuration = 0.5f;

    private AudioClip _requestedClip;
    private bool _isSwitchingMusic = false;

    public void SwitchClip(AudioClip music)
    {
        _requestedClip = music;
        if (_requestedClip != null && _audioSource.clip != null && _requestedClip.GetInstanceID() == _audioSource.clip.GetInstanceID())
        {
            return;
        }
        if (_isSwitchingMusic)
        {
            return;
        }
        _isSwitchingMusic = true;
        float currentVolume = _audioSource.volume;
        Sequence volumeTweenSequence = DOTween.Sequence().OnUpdate(() =>
        {
            _audioSource.volume = currentVolume;
        });
        volumeTweenSequence.Append(DOTween.To(() => currentVolume, val => currentVolume = val, 0f, _fadeDuration)).SetUpdate(true).SetEase(Ease.InCubic);
        volumeTweenSequence.AppendCallback(() =>
        {
            _audioSource.Stop();
            _audioSource.clip = _requestedClip;
            _audioSource.Play();
        });
        volumeTweenSequence.Append(DOTween.To(() => currentVolume, val => currentVolume = val, 1f, _fadeDuration)).SetUpdate(true).SetEase(Ease.InCubic);
        volumeTweenSequence.AppendCallback(() =>
        {
            _isSwitchingMusic = false;
            SwitchClip(_requestedClip);
        });
    }

    public void FadePlay(AudioClip clip)
    {
        _audioSource.clip = clip;
        _requestedClip = clip;
        _audioSource.Play();
        float currentVolume = _audioSource.volume;
        DOTween.To(() => currentVolume, val => currentVolume = val, 1f, _fadeDuration).SetEase(Ease.InCubic).OnUpdate(() =>
        {
            _audioSource.volume = currentVolume;
        });
    }

    public void FadeMute()
    {
        float currentVolume = _audioSource.volume;
        DOTween.To(() => currentVolume, val => currentVolume = val, 0f, _fadeDuration).SetEase(Ease.InCubic).OnUpdate(() =>
        {
            _audioSource.volume = currentVolume;
        });
    }
}
