using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[InitializeOnLoad]
public class SceneWizard_Welcome
{
	static SceneWizard_Welcome()
    {
		EditorApplication.update += Update;
	}

	static void Update()
	{
		EditorApplication.update -= Update;

		var showAtStartup = false;
		if (!EditorPrefs.HasKey("SW_Editor_WelcomeWindow"))
		{
			showAtStartup = true;
			EditorPrefs.SetBool("SW_Editor_WelcomeWindow", true);
		}
		else
		{
			if (Time.realtimeSinceStartup < 10)
			{
				showAtStartup = EditorPrefs.GetBool("SW_Editor_WelcomeWindow", true);
			}
		}

		if (showAtStartup)
			SceneWizard_WelcomeWindow.Init();

	}
}

public class SceneWizard_WelcomeWindow : EditorWindow
{
	Texture2D welcomeBanner;

	bool _showAtStartup;

	private static readonly string GithubURL = "https://github.com/elvismd/scene_wizard";
	private static readonly string AssetURL = "https://assetstore.unity.com/packages/tools/utilities/scene-wizard-211088";
	private static readonly string PublisherURL = "https://assetstore.unity.com/publishers/29558";
	private static readonly string WebsiteURL = "https://elvismd.com/";
	private static readonly string TwitterURL = "https://twitter.com/elvismdd";

	private static readonly string Asset1URL = "https://assetstore.unity.com/packages/tools/ai/fish-flock-100717";
	private static readonly string Asset2URL = "https://assetstore.unity.com/packages/tools/level-design/object-scatter-187985";

	public static void Init()
    {
		SceneWizard_WelcomeWindow window = (SceneWizard_WelcomeWindow)EditorWindow.GetWindow(typeof(SceneWizard_WelcomeWindow), true, "Scene Wizard - Thanks for downloading!");
		window.maxSize = new Vector2(390f, 260f);
		window.minSize = window.maxSize;
	}

	GUIStyle guiStyleWelcome;
	GUIStyle guiStyleLinkButton;

	float assetTextureHeight = 120;
	Texture assetTxt1, assetTxt2;

	private void OnEnable()
    {
		string scriptLocation = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
		var sourcePath = scriptLocation.Replace("/Editor/SceneWizard_WelcomeWindow.cs", "");
		var editorGuiPath = sourcePath + "/Textures";

		welcomeBanner = AssetDatabase.LoadAssetAtPath<Texture2D>(editorGuiPath + "/welcome tab.png");
		
		guiStyleWelcome = null;
		_showAtStartup = EditorPrefs.GetBool("SW_Editor_WelcomeWindow", true);

		if (assetTxt1 == null)
		{
			SetupCoroutineEditor(GetTexture("https://assetstorev1-prd-cdn.unity3d.com/key-image/a68eeec4-4476-44a3-b1cd-29c092cfbedd.jpg", (Texture texture) =>
			{
				assetTxt1 = texture;
				minSize = maxSize = new Vector2(maxSize.x, maxSize.y + assetTextureHeight);
			}));
		}

		if (assetTxt2 == null)
		{
			SetupCoroutineEditor(GetTexture("https://assetstorev1-prd-cdn.unity3d.com/key-image/65aaf265-34c3-4e60-b019-6741390d5492.jpg", (Texture texture) =>
			{
				assetTxt2 = texture;
			}));
		}
	}

    void OnGUI()
	{
		if (guiStyleWelcome == null)
		{
			guiStyleWelcome = new GUIStyle(GUI.skin.label);
			guiStyleWelcome.alignment = TextAnchor.MiddleCenter;
			guiStyleWelcome.fontSize = 10;
		}

		if (guiStyleLinkButton == null)
		{
			guiStyleLinkButton = new GUIStyle(GUI.skin.label);
			guiStyleLinkButton.normal.textColor = new Color(0.2980392f, 0.4901961f, 1f);
			guiStyleLinkButton.hover.textColor = Color.white;
			guiStyleLinkButton.active.textColor = Color.grey;
		}

		EditorGUILayout.BeginVertical();

		EditorGUILayout.Space();
		//GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		//GUILayout.FlexibleSpace();
		
		GUILayout.Label(new GUIContent(welcomeBanner), guiStyleWelcome, GUILayout.MaxWidth(Screen.width), GUILayout.MaxHeight(53));

		//GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		//GUILayout.FlexibleSpace();

		EditorGUILayout.LabelField("Thank you for downloading Scene Wizard! \n" +
			"If you find it useful please leave a feedback!", EditorStyles.wordWrappedLabel);

		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		GUILayout.Space(5);
		if (GUILayout.Button("Github", guiStyleLinkButton, GUILayout.MaxWidth(Screen.width * 0.12f)))
        {
			Application.OpenURL(GithubURL);
		}
		GUILayout.Label("-", GUILayout.MaxWidth(15));
		if (GUILayout.Button("Asset Store", guiStyleLinkButton, GUILayout.MaxWidth(Screen.width * 0.2f)))
		{
			Application.OpenURL(AssetURL);
		}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField(
			"If you want to support me so I can continue releasing tools and assets like this one, you can checkout my paid assets in the links below:" +
			"", EditorStyles.wordWrappedLabel);

		EditorGUILayout.Space(15);
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("More Assets"))
		{
			Application.OpenURL(PublisherURL);
		}
		if(GUILayout.Button("Website"))
        {
			Application.OpenURL(WebsiteURL);
		}
		if (GUILayout.Button("Twitter"))
		{
			Application.OpenURL(TwitterURL);
		}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space(10);
		EditorGUILayout.BeginHorizontal();
		if (assetTxt1 != null)
        {
			//GUILayout.Label(new GUIContent(assetTxt1), welcomeStyle, GUILayout.MaxWidth(Screen.width), GUILayout.MaxHeight(assetTextureHeight));
			if (GUILayout.Button(new GUIContent(assetTxt1), guiStyleWelcome, GUILayout.MaxWidth(Screen.width), GUILayout.MaxHeight(assetTextureHeight)))
			{
				Application.OpenURL(Asset1URL);
			}
		}

		if (assetTxt2 != null)
		{
			//GUILayout.Label(new GUIContent(assetTxt2), welcomeStyle, GUILayout.MaxWidth(Screen.width), GUILayout.MaxHeight(assetTextureHeight));
			if(GUILayout.Button(new GUIContent(assetTxt2), guiStyleWelcome, GUILayout.MaxWidth(Screen.width), GUILayout.MaxHeight(assetTextureHeight)))
            {
				Application.OpenURL(Asset2URL);
            }
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space(12);

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		EditorGUI.BeginChangeCheck();
		_showAtStartup = (GUILayout.Toggle(_showAtStartup, "Show at Startup"));
		if(EditorGUI.EndChangeCheck())
        {
			EditorPrefs.SetBool("SW_Editor_WelcomeWindow", _showAtStartup);
		}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
	}

	IEnumerator GetTexture(string url, Action<Texture> onEnd = null)
	{
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
		yield return www.SendWebRequest();

		while (www.result == UnityWebRequest.Result.InProgress) yield return null;
		
		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
		}
		else
		{
			var result = ((DownloadHandlerTexture)www.downloadHandler).texture;
			if (onEnd != null)
				onEnd(result);
		}
	}

	static void SetupCoroutineEditor(IEnumerator coroutine)
	{
		EditorApplication.CallbackFunction func = null;

		func = () =>
		{
			try
			{
				if (coroutine.MoveNext() == false) EditorApplication.update -= func;			
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				EditorApplication.update -= func;
			}
		};

		EditorApplication.update += func;
	}
}