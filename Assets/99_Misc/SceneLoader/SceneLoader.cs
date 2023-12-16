using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private string _requestSceneName;
    [SerializeField]
    private float _loadDelay = 1f;

    [Header("Reference - Read/Write")]
    [SerializeField]
    private StringVariable _requestLoadScene;
    [SerializeField]
    private FloatVariable _loadProgress;

    [SerializeField]
    private UnityEvent _onReceivedLoadRequest;
    [SerializeField]
    private UnityEvent _onStartLoad;

    public void LoadRequestedScene()
    {
        LoadRequestedScene(_requestSceneName).Forget();
    }

    public void LoadScene(string sceneName)
    {
        _requestSceneName = sceneName;
        LoadRequestedScene();
    }

    public async UniTaskVoid LoadRequestedScene(string sceneName)
    {
        _onReceivedLoadRequest.Invoke();
        await UniTask.Delay(System.TimeSpan.FromSeconds(_loadDelay), ignoreTimeScale: true);
        _onStartLoad.Invoke();

        _requestLoadScene.Value = sceneName;
        _loadProgress.Value = 0f;

        // BufferScene is responsible for loading target scene
        var loadOperation = SceneManager.LoadSceneAsync("BufferScene");

    WaitForLoadingScene:
        await UniTask.NextFrame();
        _loadProgress.Value = loadOperation.progress / 2f;
        if (loadOperation.progress < 1f)
        {
            goto WaitForLoadingScene;
        }
    }

    public void BufferSceneOnly_LoadScene()
    {
        Async_BufferSceneLoadNextScene().Forget();
    }
    private async UniTaskVoid Async_BufferSceneLoadNextScene()
    {
        var loadOperation = SceneManager.LoadSceneAsync(_requestLoadScene.Value);

    WaitForLoadingScene:
        await UniTask.NextFrame();
        _loadProgress.Value += loadOperation.progress / 2f;
        if (loadOperation.progress < 1f)
        {
            goto WaitForLoadingScene;
        }

        _loadProgress.Value = 1f;
    }
}
