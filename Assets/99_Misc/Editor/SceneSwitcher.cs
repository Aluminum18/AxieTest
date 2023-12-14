using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneSwitcher : EditorWindow
{
    private const string FAVORITES = "FavoriteScenes";

    private List<SceneAsset> _favoriteScenes = new();
    private List<SceneAsset> _allSceneAssets = new();
    private Vector3 _scrollPos;

    [MenuItem("Tools/SceneSwitcher")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SceneSwitcher), false, "SceneSwitcher");
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Remove EditorPrefs"))
        {
            EditorPrefs.DeleteAll();
        }

        if (GUILayout.Button("Refresh"))
        {
            LoadSceneList();
        }

        GUILayout.Space(20f);

        GUILayout.Label("Favorite", EditorStyles.boldLabel);

        if (_favoriteScenes == null)
        {
            return;
        }
        
        for (int i = 0; i < _favoriteScenes.Count; i++)
        {
            var sceneAsset = _favoriteScenes[i];
            if (sceneAsset == null)
            {
                continue;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(sceneAsset.name))
            {
                OpenScene(sceneAsset);
            }
            
            if (GUILayout.Button("Remove From Favorite", GUILayout.MaxWidth(200f)))
            {
                RemoveFromFavorite(sceneAsset);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20f);
        GUILayout.Label("All Scenes", EditorStyles.boldLabel);

        if (_allSceneAssets == null)
        {
            return;
        }


        _scrollPos = GUILayout.BeginScrollView(_scrollPos);
        for (int i = 0; i < _allSceneAssets.Count; i++)
        {
            SceneAsset scene = _allSceneAssets[i];

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(scene.name))
            {
                OpenScene(scene);
            }

            GUI.enabled = !_favoriteScenes.Contains(scene);
            if (GUILayout.Button("Add To Favorite", GUILayout.MaxWidth(200f)))
            {
                AddToFavorite(scene);
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

        }
        GUILayout.EndScrollView();
    }

    private void LoadSceneList()
    {
        string favoriteScenesStr = EditorPrefs.GetString(FAVORITES);
        string[] favoriteSceneGuids = favoriteScenesStr.Split(',');
        for (int i = 0; i < favoriteSceneGuids.Length; i++)
        {
            string guid = favoriteSceneGuids[i];
            if (string.IsNullOrEmpty(guid))
            {
                continue;
            }

            var scene = GetSceneAsset(guid);
            if (_favoriteScenes.Contains(scene))
            {
                continue;
            }

            _favoriteScenes.Add(GetSceneAsset(guid));
        }

        string[] allSceneGuids = AssetDatabase.FindAssets("t:scene");
        _allSceneAssets.Clear();
        for (int i = 0; i < allSceneGuids.Length; i++)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(allSceneGuids[i]);
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            _allSceneAssets.Add(scene);
        }

        _allSceneAssets.Sort(Compare);
    }

    private SceneAsset GetSceneAsset(string guid)
    {
        string scenePath = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
    }

    private void AddToFavorite(SceneAsset scene)
    {
        if (IsContainedInFavorite(scene))
        {
            return;
        }

        string favoriteGuids = EditorPrefs.GetString(FAVORITES);
        string path = AssetDatabase.GetAssetPath(scene);
        string guid = AssetDatabase.AssetPathToGUID(path);
        EditorPrefs.SetString(FAVORITES, $"{favoriteGuids},{guid}");

        if (_favoriteScenes.Contains(scene))
        {
            return;
        }
        _favoriteScenes.Add(scene);
    }

    private void RemoveFromFavorite(SceneAsset scene)
    {
        string path = AssetDatabase.GetAssetPath(scene);
        string guid = AssetDatabase.AssetPathToGUID(path);

        string[] favoriteGuids = EditorPrefs.GetString(FAVORITES).Split(',');
        StringBuilder sb = new();
        for (int i = 0; i < favoriteGuids.Length; i++)
        {
            string favorite = favoriteGuids[i];
            if (favorite == guid)
            {
                continue;
            }

            sb.Append($"{favorite},");
        }
        EditorPrefs.SetString(FAVORITES, sb.ToString());

        _favoriteScenes.Remove(scene);
    }

    private bool IsContainedInFavorite(SceneAsset scene)
    {
        string favoriteScenesStr = EditorPrefs.GetString(FAVORITES);
        string[] favoriteSceneGuids = favoriteScenesStr.Split(',');
        string path = AssetDatabase.GetAssetPath(scene);
        string targetGuid = AssetDatabase.AssetPathToGUID(path);

        for (int i = 0; i < favoriteSceneGuids.Length; i++)
        {
            string guid = favoriteSceneGuids[i];
            if (guid == targetGuid)
            {
                return true;
            }
        }

        return false;
    }

    private void OpenScene(SceneAsset scene)
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }
        EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene));
    }

    private int Compare(SceneAsset a, SceneAsset b)
    {
        return string.Compare(a.name, b.name);
    }
}
