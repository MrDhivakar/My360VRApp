using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class BuildScript
{
    private const string ScenePath = "Assets/Scenes/Main.unity";
    private const string PackageName = "com.nanba.memories360";

    public static void BuildAndroid()
    {
        EnsureSceneExists();

        EditorBuildSettings.scenes = new[] {
            new EditorBuildSettingsScene(ScenePath, true)
        };

        PlayerSettings.SetApplicationIdentifier(
            BuildTargetGroup.Android, PackageName);
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.productName = "360 Memories";
        PlayerSettings.companyName = "Nanba";

        PlayerSettings.SetScriptingBackend(
            BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        System.IO.Directory.CreateDirectory("Builds/Android");

        var options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = "Builds/Android/My360VRApp.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.LogError("Build FAILED: " + report.summary.result);
            EditorApplication.Exit(1);
        }
        Debug.Log("Build SUCCEEDED: " + report.summary.outputPath);
    }

    private static void EnsureSceneExists()
    {
        if (System.IO.File.Exists(ScenePath)) return;

        Debug.Log("Creating Main scene programmatically...");
        System.IO.Directory.CreateDirectory("Assets/Scenes");

        var scene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // --- Camera ---
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        camGO.tag = "MainCamera";

        // --- Panorama Sphere ---
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "PanoramaSphere";
        sphere.transform.localScale = new Vector3(-1, 1, 1);
        Object.DestroyImmediate(sphere.GetComponent<Collider>());
        var shader = Shader.Find("Unlit/Texture");
        if (shader != null)
        {
            var mat = new Material(shader) { name = "PanoramaMat" };
            sphere.GetComponent<Renderer>().material = mat;
        }

        // --- Managers ---
        var memGO = new GameObject("MemoryManager");
        memGO.AddComponent<MemoryManager>();

        var viewerGO = new GameObject("PanoramaViewer");
        var viewer = viewerGO.AddComponent<PanoramaViewer>();
        viewer.panoramaSphere = sphere;

        // --- Canvas (UI) ---
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // MemoryListPanel (ScrollView simplified)
        var listPanel = CreatePanel(canvasGO.transform, "MemoryListPanel",
            new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.9f));

        var scrollGO = new GameObject("ScrollView", typeof(RectTransform));
        scrollGO.transform.SetParent(listPanel.transform, false);
        var scrollRT = scrollGO.GetComponent<RectTransform>();
        scrollRT.anchorMin = Vector2.zero;
        scrollRT.anchorMax = Vector2.one;
        scrollRT.offsetMin = Vector2.zero;
        scrollRT.offsetMax = Vector2.zero;

        var scrollRect = scrollGO.AddComponent<ScrollRect>();
        var viewportGO = new GameObject("Viewport", typeof(RectTransform));
        viewportGO.transform.SetParent(scrollGO.transform, false);
        var viewportRT = viewportGO.GetComponent<RectTransform>();
        viewportRT.anchorMin = Vector2.zero;
        viewportRT.anchorMax = Vector2.one;
        viewportRT.offsetMin = Vector2.zero;
        viewportRT.offsetMax = Vector2.zero;

        var contentGO = new GameObject("Content", typeof(RectTransform));
        contentGO.transform.SetParent(viewportGO.transform, false);
        var contentRT = contentGO.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0.5f, 1f);
        contentRT.sizeDelta = new Vector2(0, 0);

        scrollRect.viewport = viewportRT;
        scrollRect.content = contentRT;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        var layout = contentGO.AddComponent<VerticalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;

        // MemoryItem prefab (Button + Text)
        var itemGO = new GameObject("MemoryItem", typeof(RectTransform));
        itemGO.transform.SetParent(null);
        var itemRT = itemGO.GetComponent<RectTransform>();
        itemRT.sizeDelta = new Vector2(0, 80);
        var img = itemGO.AddComponent<Image>();
        img.color = new Color(0.2f, 0.4f, 0.8f, 0.8f);
        var btn = itemGO.AddComponent<Button>();
        var memBtn = itemGO.AddComponent<MemoryButton>();

        var txtGO = new GameObject("Title", typeof(RectTransform));
        txtGO.transform.SetParent(itemGO.transform, false);
        var txtRT = txtGO.GetComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.offsetMin = new Vector2(20, 0);
        txtRT.offsetMax = new Vector2(-20, 0);
        var text = txtGO.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 32;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
        memBtn.titleText = text;

        // Prefab
        System.IO.Directory.CreateDirectory("Assets/Prefabs");
        var prefab = PrefabUtility.SaveAsPrefabAsset(itemGO,
            "Assets/Prefabs/MemoryItem.prefab");
        Object.DestroyImmediate(itemGO);

        // ViewerPanel (empty full-screen panel, initially hidden)
        var viewerPanel = CreatePanel(canvasGO.transform, "ViewerPanel",
            Vector2.zero, Vector2.one);
        viewerPanel.SetActive(false);

        // UIManager
        var uiManager = canvasGO.AddComponent<UIManager>();
        uiManager.memoryListPanel = listPanel;
        uiManager.contentParent = contentGO.transform;
        uiManager.memoryItemPrefab = prefab;
        uiManager.viewerPanel = viewerPanel;
        uiManager.panoramaViewer = viewer;

        // EventSystem
        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<StandaloneInputModule>();

        EditorSceneManager.SaveScene(scene, ScenePath);
        Debug.Log("Scene created at: " + ScenePath);
    }

    private static GameObject CreatePanel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }
}
