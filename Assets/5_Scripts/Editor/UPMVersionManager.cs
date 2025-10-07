using UnityEngine;
using UnityEditor;
using System.IO;

namespace RedMinS.Editor
{
    /// <summary>
    /// Unity Package Manager 버전 관리 도구
    /// </summary>
    public class UPMVersionManager : EditorWindow
    {
        private string packageName = "com.redmins.core-framework";
        private string packageDisplayName = "RedMinS Core Framework";
        private string packageVersion = "1.0.0";
        private string authorName = "RedMinS";
        private string repositoryUrl = "https://github.com/redredmins/unity-core-framework.git";

        [MenuItem("RedMinS/Package Manager/Version Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<UPMVersionManager>("UPM Version Manager");
            window.minSize = new Vector2(450, 600);
            window.Show();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawPackageSettings();
            DrawMigrationButtons();
            DrawGitInstructions();
        }

        private void DrawHeader()
        {
            GUILayout.Space(10);
            var titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
            EditorGUILayout.LabelField("Unity Package Manager - Version Control", titleStyle);
            EditorGUILayout.LabelField("Convert to UPM with Git versioning", EditorStyles.centeredGreyMiniLabel);
            GUILayout.Space(10);
            EditorGUILayout.Separator();
        }

        private void DrawPackageSettings()
        {
            EditorGUILayout.LabelField("📦 Package Configuration", EditorStyles.boldLabel);
            packageName = EditorGUILayout.TextField("Package Name", packageName);
            packageDisplayName = EditorGUILayout.TextField("Display Name", packageDisplayName);
            packageVersion = EditorGUILayout.TextField("Version", packageVersion);
            authorName = EditorGUILayout.TextField("Author", authorName);
            repositoryUrl = EditorGUILayout.TextField("Repository URL", repositoryUrl);
            GUILayout.Space(10);
        }

        private void DrawMigrationButtons()
        {
            EditorGUILayout.LabelField("🚀 Migration Actions", EditorStyles.boldLabel);
            
            if (GUILayout.Button("1. Create Package Structure", GUILayout.Height(30)))
            {
                CreatePackageStructure();
            }
            
            if (GUILayout.Button("2. Generate Package Files", GUILayout.Height(30)))
            {
                GeneratePackageFiles();
            }
            
            if (GUILayout.Button("3. Copy Scripts to Package", GUILayout.Height(30)))
            {
                CopyScriptsToPackage();
            }
            
            GUILayout.Space(10);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("🎯 Complete Migration", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("Complete Migration", 
                    "Create complete UPM package?\n\nBackup your project first!", 
                    "Yes", "Cancel"))
                {
                    CompleteMigration();
                }
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawGitInstructions()
        {
            GUILayout.Space(15);
            EditorGUILayout.LabelField("📋 Git Setup Instructions", EditorStyles.boldLabel);
            
            EditorGUILayout.HelpBox(
                "After migration:\n" +
                "1. cd Packages/" + packageName + "\n" +
                "2. git init\n" +
                "3. git add .\n" +
                "4. git commit -m \"Initial release\"\n" +
                "5. git remote add origin YOUR_REPO_URL\n" +
                "6. git push -u origin main\n" +
                "7. git tag v" + packageVersion + "\n" +
                "8. git push origin v" + packageVersion, 
                MessageType.Info);
            
            if (GUILayout.Button("Copy Git Commands"))
            {
                CopyGitCommands();
            }
            
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Package Manager URL:", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel(repositoryUrl + ".git", EditorStyles.textField, GUILayout.Height(20));
        }

        private void CreatePackageStructure()
        {
            string packagePath = Path.Combine("Packages", packageName);
            
            string[] dirs = {
                packagePath,
                Path.Combine(packagePath, "Runtime"),
                Path.Combine(packagePath, "Runtime", "Core"),
                Path.Combine(packagePath, "Runtime", "SceneManagement"),
                Path.Combine(packagePath, "Runtime", "UI"),
                Path.Combine(packagePath, "Runtime", "Module"),
                Path.Combine(packagePath, "Editor"),
                Path.Combine(packagePath, "Samples~", "BasicSetup"),
                Path.Combine(packagePath, "Documentation~")
            };
            
            foreach (string dir in dirs)
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            
            AssetDatabase.Refresh();
            Debug.Log("✅ Package structure created!");
            EditorUtility.DisplayDialog("Success", "Package structure created!", "OK");
        }

        private void GeneratePackageFiles()
        {
            string packagePath = Path.Combine("Packages", packageName);
            
            // package.json
            string packageJson = CreatePackageJson();
            File.WriteAllText(Path.Combine(packagePath, "package.json"), packageJson);
            
            // README.md
            string readme = CreateReadme();
            File.WriteAllText(Path.Combine(packagePath, "README.md"), readme);
            
            // CHANGELOG.md
            string changelog = CreateChangelog();
            File.WriteAllText(Path.Combine(packagePath, "CHANGELOG.md"), changelog);
            
            // Assembly definitions
            CreateAssemblyDefinitions(packagePath);
            
            AssetDatabase.Refresh();
            Debug.Log("✅ Package files generated!");
            EditorUtility.DisplayDialog("Success", "Package files generated successfully!", "OK");
        }

        private void CopyScriptsToPackage()
        {
            string packagePath = Path.Combine("Packages", packageName);
            string sourcePath = "Assets/5_Scripts";
            
            // Copy directories
            if (Directory.Exists(Path.Combine(sourcePath, "Core")))
                CopyDirectory(Path.Combine(sourcePath, "Core"), Path.Combine(packagePath, "Runtime", "Core"));
            
            if (Directory.Exists(Path.Combine(sourcePath, "SceneManagement")))
                CopyDirectory(Path.Combine(sourcePath, "SceneManagement"), Path.Combine(packagePath, "Runtime", "SceneManagement"));
            
            if (Directory.Exists(Path.Combine(sourcePath, "UI")))
                CopyDirectory(Path.Combine(sourcePath, "UI"), Path.Combine(packagePath, "Runtime", "UI"));
            
            if (Directory.Exists(Path.Combine(sourcePath, "Module")))
                CopyDirectory(Path.Combine(sourcePath, "Module"), Path.Combine(packagePath, "Runtime", "Module"));
            
            if (Directory.Exists(Path.Combine(sourcePath, "Editor")))
                CopyDirectory(Path.Combine(sourcePath, "Editor"), Path.Combine(packagePath, "Editor"));
            
            if (Directory.Exists(Path.Combine(sourcePath, "Examples")))
                CopyDirectory(Path.Combine(sourcePath, "Examples"), Path.Combine(packagePath, "Samples~", "BasicSetup"));
            
            AssetDatabase.Refresh();
            Debug.Log("✅ Scripts copied to package!");
            EditorUtility.DisplayDialog("Success", "Scripts copied successfully!", "OK");
        }

        private void CompleteMigration()
        {
            Debug.Log("🚀 Starting complete UPM migration...");
            
            CreatePackageStructure();
            GeneratePackageFiles();
            CopyScriptsToPackage();
            
            string message = $@"🎉 UPM Migration Complete!

Package created at: Packages/{packageName}

Next steps:
1. Navigate to the package folder
2. Initialize Git repository
3. Push to GitHub
4. Install via Package Manager

Copy Git commands?";

            if (EditorUtility.DisplayDialog("Migration Complete!", message, "Copy Git Commands", "Close"))
            {
                CopyGitCommands();
            }
            
            Debug.Log("🎉 UPM Migration completed successfully!");
        }

        private void CopyGitCommands()
        {
            string commands = $@"# Git setup for {packageDisplayName}
cd Packages/{packageName}
git init
git add .
git commit -m ""feat: Initial {packageDisplayName} v{packageVersion}""
git remote add origin {repositoryUrl}.git
git push -u origin main
git tag v{packageVersion}
git push origin v{packageVersion}

# Install in Unity:
# Package Manager → + → Add from Git URL → {repositoryUrl}.git";

            EditorGUIUtility.systemCopyBuffer = commands;
            Debug.Log("Git commands copied to clipboard!");
            EditorUtility.DisplayDialog("Copied!", "Git commands copied to clipboard!", "OK");
        }

        // Content generation methods
        private string CreatePackageJson()
        {
            return $@"{{
  ""name"": ""{packageName}"",
  ""version"": ""{packageVersion}"",
  ""displayName"": ""{packageDisplayName}"",
  ""description"": ""Complete Unity framework with Scene Management, UI System, Object Pooling, and Core Architecture patterns."",
  ""unity"": ""2021.3"",
  ""dependencies"": {{
    ""com.unity.textmeshpro"": ""3.0.6""
  }},
  ""keywords"": [""framework"", ""architecture"", ""scene-management"", ""ui-system"", ""object-pooling""],
  ""author"": {{
    ""name"": ""{authorName}"",
    ""url"": ""{repositoryUrl}""
  }},
  ""type"": ""library"",
  ""samples"": [
    {{
      ""displayName"": ""Basic Setup"",
      ""description"": ""Basic scene setup with Core Framework"",
      ""path"": ""Samples~/BasicSetup""
    }}
  ]
}}";
        }

        private string CreateReadme()
        {
            return $@"# {packageDisplayName}

Complete Unity project framework for rapid game development.

## 🚀 Features

- **Core Architecture**: Centralized manager system
- **Scene Management**: Advanced loading and transitions
- **UI System**: Modular popups, toasts, and effects
- **Object Pooling**: High-performance object reuse

## 📦 Installation

### Package Manager
1. Open Package Manager in Unity
2. Click '+' → 'Add package from git URL'
3. Enter: `{repositoryUrl}.git`

## 🛠️ Quick Start

```csharp
// Access core systems
var ui = Core.app.ui;
var sceneManager = AdvancedSceneManager.Instance;

// Show toast
ui.ShowToast(""Welcome!"");

// Load scene
sceneManager.LoadScene(""GameScene"");
```

## 📄 License

MIT License - see LICENSE.md

---
Made with ❤️ by {authorName}";
        }

        private string CreateChangelog()
        {
            return $@"# Changelog

## [{packageVersion}] - {System.DateTime.Now.ToString("yyyy-MM-dd")}

### Added
- Initial release of {packageDisplayName}
- Core Framework with centralized access
- Advanced Scene Management system
- Modular UI System with popups and toasts
- High-performance Object Pooling
- Editor tools for automated setup

### Features
- Thread-safe Singleton patterns
- Async scene loading with progress
- Customizable scene transitions
- Stack-based popup management
- Queue-based toast notifications
- Generic and prefab object pools";
        }

        private void CreateAssemblyDefinitions(string packagePath)
        {
            // Runtime asmdef
            string runtimeAsmdef = @"{
    ""name"": ""RedMinS.Runtime"",
    ""rootNamespace"": ""RedMinS"",
    ""references"": [""Unity.TextMeshPro""],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [
        {
            ""name"": ""com.unity.textmeshpro"",
            ""expression"": ""3.0.0"",
            ""define"": ""TMP_PRESENT""
        }
    ],
    ""noEngineReferences"": false
}";
            File.WriteAllText(Path.Combine(packagePath, "Runtime", "RedMinS.Runtime.asmdef"), runtimeAsmdef);

            // Editor asmdef
            string editorAsmdef = @"{
    ""name"": ""RedMinS.Editor"",
    ""rootNamespace"": ""RedMinS.Editor"",
    ""references"": [""RedMinS.Runtime""],
    ""includePlatforms"": [""Editor""],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}";
            File.WriteAllText(Path.Combine(packagePath, "Editor", "RedMinS.Editor.asmdef"), editorAsmdef);
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(targetDir, fileName), true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(directory);
                CopyDirectory(directory, Path.Combine(targetDir, dirName));
            }
        }
    }
}