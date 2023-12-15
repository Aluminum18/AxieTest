using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoadTransition : MonoSingleton<SceneLoadTransition>
{
    [SerializeField]
    private UnityEvent _onEnable;
    [SerializeField]
    private UnityEvent _onSceneLoaded;

    protected override void DoOnEnable()
    {
        _onEnable.Invoke();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void DoOnDisable()
    {
        base.DoOnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadMode)
    {
        UniTask_OnSceneLoaded(loadedScene, loadMode).Forget();
    }

    private async UniTaskVoid UniTask_OnSceneLoaded(Scene loadedScene, LoadSceneMode loadMode)
    {
        if (loadedScene.name == "BufferScene")
        {
            return;
        }

        await UniTask.DelayFrame(2);
        _onSceneLoaded.Invoke();
    }
}
