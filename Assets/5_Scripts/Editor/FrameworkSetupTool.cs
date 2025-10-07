using UnityEngine;
using UnityEditor;

namespace RedMinS.Editor
{
    /// <summary>
    /// RedMinS Core Framework 에디터 도구
    /// </summary>
    public class FrameworkSetupTool : EditorWindow
    {
        [MenuItem("RedMinS/Framework Setup Tool")]
        public static void ShowWindow()
        {
            var window = GetWindow<FrameworkSetupTool>("Framework Setup");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawQuickActions();
        }

        private void DrawHeader()
        {
            GUILayout.Space(10);
            
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter
            };
            
            EditorGUILayout.LabelField("RedMinS Core Framework", titleStyle);
            EditorGUILayout.LabelField("Version 1.0.0", EditorStyles.centeredGreyMiniLabel);
            
            GUILayout.Space(10);
            EditorGUILayout.Separator();
        }

        private void DrawQuickActions()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Setup Basic Framework Scene"))
            {
                SetupBasicFrameworkScene();
            }
            
            if (GUILayout.Button("Export as Unity Package"))
            {
                ExportAsUnityPackage();
            }
            
            if (GUILayout.Button("Generate Package Files"))
            {
                GeneratePackageFiles();
            }
        }

        private void SetupBasicFrameworkScene()
        {
            // CoreBootstrap 생성
            var bootstrapGO = new GameObject("[Framework] CoreBootstrap");
            bootstrapGO.AddComponent<RedMinS.CoreBootstrap>();
            
            // UI Canvas 설정
            var canvasGO = new GameObject("[Framework] UI Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            canvasGO.AddComponent<RedMinS.UI.UIManager>();
            
            // EventSystem 추가
            var eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            Debug.Log("[FrameworkSetupTool] Basic framework scene setup completed!");
            EditorUtility.DisplayDialog("Success", "Basic framework scene has been set up successfully!", "OK");
        }

        private void ExportAsUnityPackage()
        {
            string[] assetPaths = {
                "Assets/5_Scripts/Core",
                "Assets/5_Scripts/SceneManagement", 
                "Assets/5_Scripts/UI",
                "Assets/5_Scripts/Module",
                "Assets/5_Scripts/Examples"
            };
            
            string fileName = EditorUtility.SaveFilePanel(
                "Export Framework Package",
                "",
                "RedMinSCoreFramework.unitypackage",
                "unitypackage"
            );
            
            if (!string.IsNullOrEmpty(fileName))
            {
                AssetDatabase.ExportPackage(assetPaths, fileName, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
                Debug.Log($"[FrameworkSetupTool] Package exported to: {fileName}");
                EditorUtility.DisplayDialog("Success", $"Package exported successfully!", "OK");
            }
        }

        private void GeneratePackageFiles()
        {
            Debug.Log("[FrameworkSetupTool] Use the template files in Examples folder to create package.json and README.md");
            EditorUtility.DisplayDialog("Info", "Please refer to the template files in the Examples folder for package.json and README.md creation.", "OK");
        }
    }

    /// <summary>
    /// 프로젝트 설정 자동화 도구
    /// </summary>
    public static class ProjectSetupAutomation
    {
        [MenuItem("RedMinS/Setup/Quick Setup New Project")]
        public static void QuickSetupNewProject()
        {
            if (EditorUtility.DisplayDialog(
                "Quick Setup", 
                "This will setup a new project with RedMinS Core Framework.\n\n" +
                "This will:\n" +
                "• Create CoreBootstrap\n" +
                "• Setup UI Canvas\n" +
                "• Add EventSystem\n" +
                "• Configure basic scene\n\n" +
                "Continue?", 
                "Yes", "Cancel"))
            {
                // 기본 설정 실행
                var bootstrap = new GameObject("[Framework] CoreBootstrap").AddComponent<RedMinS.CoreBootstrap>();
                
                var canvasGO = new GameObject("[Framework] UI Canvas");
                var canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                canvasGO.AddComponent<RedMinS.UI.UIManager>();
                
                var eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                
                Debug.Log("[ProjectSetupAutomation] Quick setup completed successfully!");
                EditorUtility.DisplayDialog("Success", "New project setup completed!\n\nYou can now start building your game with the RedMinS Core Framework.", "OK");
            }
        }
    }
}