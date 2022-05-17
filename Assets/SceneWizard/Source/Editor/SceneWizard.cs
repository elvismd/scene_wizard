using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SceneWizard : EditorWindow
{
    SceneWizardConfig config;

    Vector2 scrollView;

    [MenuItem("Window/EMD Tools/Scene Wizard")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SceneWizard window = (SceneWizard)EditorWindow.GetWindow(typeof(SceneWizard));
        window.titleContent = new GUIContent("Scene Wizard", new GUIContent(EditorGUIUtility.IconContent("d_UnityLogo")).image);
        window.Show();
    }

    void RefreshConfig()
    {
        if (config == null)
        {
            bool sucessFindingConfig = false;

            if (!File.Exists(Application.dataPath + "/SceneWizard_Config.asset"))
            {
                SceneWizardConfig newConfig = new SceneWizardConfig();
	            AssetDatabase.CreateAsset(newConfig, "Assets/SceneWizard_Config.asset");
                
                AssetDatabase.SaveAssets();

                AssetDatabase.Refresh();
                
            }

            var foundAssets = AssetDatabase.FindAssets("SceneWizard_Config", new[] { "Assets/" });
            if (foundAssets.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(foundAssets[0]);
	            config = AssetDatabase.LoadAssetAtPath<SceneWizardConfig>(path);
            }

            if(sucessFindingConfig)
                EditorUtility.SetDirty(config);
        }
    }

    void ReloadScenes()
    {
        config.scenes = new List<SceneConfigSetup>();
	    LoadFromPath(config.folderPath);
        
	    
    }

    void LoadFromPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        string[] files = Directory.GetFiles(path);
        foreach (var fp in files)
        {
            if (fp.Contains(".unity"))
            {
                var assetPath = "Assets" + fp.Split("Assets")[1];

                var sceneLoaded = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);

                if (sceneLoaded != null)
                {
                    var pathSplit = assetPath.Replace("\\", "/").Split("/");

                    SceneConfigSetup scs = new SceneConfigSetup()
                    {
                        name = sceneLoaded.name,
                        path = assetPath,
                        parentFolder = pathSplit[pathSplit.Length - 2]
                    };

                    config.scenes.Add(scs);
                }
            }
        }


        if (config.allowSubfolders)
        {
            string[] dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                LoadFromPath(dir);
            }
        }

        EditorUtility.SetDirty(config);
    }

    private void OnEnable()
    {
        RefreshConfig();
        ReloadScenes();
    }

    private void OnFocus()
    {
        RefreshConfig();
        ReloadScenes();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label("Scene Wizard", EditorStyles.boldLabel);
        if(GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("_Help")).image, GUILayout.MaxWidth(30)))
        {
            SceneWizard_WelcomeWindow.Init();
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20);
        RefreshConfig();
        ReloadScenes();

        var prevAlignment = GUI.skin.button.alignment;
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        if (config.folderPath == "" || config.folderPath == null)
        {
            if (GUILayout.Button("Select a folder"))
            {
                // EditorUtility.DisplayDialog("Select Folder", "You must select a folder first!", "OK");

                string path = EditorUtility.OpenFolderPanel("Select a folder to load Scenes from", "", "");
                config.folderPath = path;

                EditorUtility.SetDirty(config);
            }
        }
        else
        {
            if (GUILayout.Button("Change folder"))
            {
                // EditorUtility.DisplayDialog("Select Folder", "You must select a folder first!", "OK");
                string path = EditorUtility.OpenFolderPanel("Select a folder to load Scenes from", "", "");

                if (!string.IsNullOrEmpty(path))
                {
                    config.folderPath = path;
                    config.scenes = new List<SceneConfigSetup>();

                    EditorUtility.SetDirty(config);
                }
            }

            if (config.scenes == null || config.scenes.Count <= 0)
            {
                ReloadScenes();
            }
            if (GUILayout.Button("Refresh"))
            {
                ReloadScenes();
            }

            if (GUILayout.Button("Clear"))
            {
                config.folderPath = "";
                config.scenes = new List<SceneConfigSetup>();
                EditorUtility.SetDirty(config);
            }
        }


        EditorGUILayout.EndHorizontal();

        config.allowSubfolders = EditorGUILayout.Toggle("Allow Subfolders", config.allowSubfolders);

        EditorGUILayout.EndVertical();


        if (config.scenes != null && config.scenes.Count > 0)
        {
            string lastFolderName = "";
            GUILayout.Space(15);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            scrollView = EditorGUILayout.BeginScrollView(scrollView, GUILayout.Height(position.height * 0.6f));

            EditorGUI.indentLevel++;
            foreach (var scene in config.scenes)
            {
                if (scene.parentFolder != lastFolderName)
                {
                    if (lastFolderName != "")
                        GUILayout.Space(8);

                    EditorGUI.indentLevel--;

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    GUILayout.Label(scene.parentFolder, EditorStyles.boldLabel);
                    EditorGUILayout.EndHorizontal();
                    lastFolderName = scene.parentFolder;
                    EditorGUI.indentLevel++;
                }

                EditorGUILayout.BeginHorizontal();

                //    EditorGUILayout.LabelField(scene.name + " : ");
                var sceneLoaded = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(GUIContent.none, sceneLoaded, typeof(SceneAsset), false);
                EditorGUI.EndDisabledGroup();


                if (GUILayout.Button("Open Single"))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path, UnityEditor.SceneManagement.OpenSceneMode.Single);
                }

                if (GUILayout.Button("Open Additively"))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                }

                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        GUI.skin.button.alignment = prevAlignment;
    }
}
