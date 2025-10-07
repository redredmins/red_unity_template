using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace RedMinS.Editor
{
    /// <summary>
    /// 프로젝트를 Package Manager용 패키지로 마이그레이션하는 도구
    /// </summary>
    public class PackageManagerMigrationTool : EditorWindow
    {
        private string packageName = "com.redmins.core-framework";
        private string packageDisplayName = "RedMinS Core Framework";
        private string packageVersion = "1.0.0";
        private string authorName = "RedMinS";
        private string authorEmail = "contact@redmins.com";
        private string repositoryUrl = "https://github.com/redmins/core-framework";
        
        private Vector2 scrollPosition;
        private bool showAdvancedSettings = false;

        [MenuItem("RedMinS/Package Manager/Migration Tool")]
        public static void ShowWindow()
        {
            var window = GetWindow<PackageManagerMigrationTool>("Package Migration");
            window.minSize = new Vector2(500, 600);
            window.Show();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            DrawPackageSettings();
            DrawMigrationActions();
            DrawInstructions();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            GUILayout.Space(10);
            
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter
            };
            
            EditorGUILayout.LabelField("Package Manager Migration Tool", titleStyle);
            EditorGUILayout.LabelField("Convert project to UPM package", EditorStyles.centeredGreyMiniLabel);
            
            GUILayout.Space(10);
            EditorGUILayout.Separator();
        }

        private void DrawPackageSettings()
        {
            EditorGUILayout.LabelField("Package Settings", EditorStyles.boldLabel);
            
            packageName = EditorGUILayout.TextField("Package Name", packageName);
            packageDisplayName = EditorGUILayout.TextField("Display Name", packageDisplayName);
            packageVersion = EditorGUILayout.TextField("Version", packageVersion);
            
            GUILayout.Space(5);
            
            authorName = EditorGUILayout.TextField("Author Name", authorName);
            authorEmail = EditorGUILayout.TextField("Author Email", authorEmail);
            repositoryUrl = EditorGUILayout.TextField("Repository URL", repositoryUrl);
            
            GUILayout.Space(10);
            
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings");
            if (showAdvancedSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Advanced settings for package configuration", MessageType.Info);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawMigrationActions()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Migration Actions", EditorStyles.boldLabel);
            
            if (GUILayout.Button("1. Create Package Structure", GUILayout.Height(30)))
            {
                CreatePackageStructure();
            }
            
            if (GUILayout.Button("2. Generate Package Files", GUILayout.Height(30)))
            {
                GeneratePackageFiles();
            }
            
            if (GUILayout.Button("3. Move Scripts to Package", GUILayout.Height(30)))
            {
                MoveScriptsToPackage();
            }
            
            if (GUILayout.Button("4. Create Assembly Definitions", GUILayout.Height(30)))
            {
                CreateAssemblyDefinitions();
            }
            
            if (GUILayout.Button("5. Setup Git Repository", GUILayout.Height(30)))
            {
                SetupGitRepository();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("🚀 Complete Migration (All Steps)", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("Complete Migration", 
                    "This will perform all migration steps. Make sure to backup your project first!\n\nContinue?", 
                    "Yes", "Cancel"))
                {
                    CompleteMigration();
                }
            }
        }

        private void DrawInstructions()
        {
            GUILayout.Space(15);
            EditorGUILayout.LabelField("Instructions", EditorStyles.boldLabel);
            
            EditorGUILayout.HelpBox(
                "1. Backup your project before migration\n" +
                "2. Configure package settings above\n" +
                "3. Run migration steps in order\n" +
                "4. Test the package in a new project\n" +
                "5. Push to Git repository\n" +
                "6. Install via Package Manager", 
                MessageType.Info);
        }

        private void CreatePackageStructure()
        {
            string packagePath = Path.Combine("Packages", packageName);
            
            string[] directories = {
                packagePath,
                Path.Combine(packagePath, "Runtime"),
                Path.Combine(packagePath, "Runtime", "Core"),
                Path.Combine(packagePath, "Runtime", "SceneManagement"),
                Path.Combine(packagePath, "Runtime", "UI"),
                Path.Combine(packagePath, "Runtime", "Module"),
                Path.Combine(packagePath, "Editor"),
                Path.Combine(packagePath, "Editor", "Tools"),
                Path.Combine(packagePath, "Tests"),
                Path.Combine(packagePath, "Tests", "Runtime"),
                Path.Combine(packagePath, "Tests", "Editor"),
                Path.Combine(packagePath, "Samples~"),
                Path.Combine(packagePath, "Samples~", "BasicSetup"),
                Path.Combine(packagePath, "Samples~", "CompleteExample"),
                Path.Combine(packagePath, "Samples~", "Benchmarks"),
                Path.Combine(packagePath, "Documentation~")
            };
            
            foreach (string dir in directories)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    Debug.Log($"Created directory: {dir}");
                }
            }
            
            AssetDatabase.Refresh();
            Debug.Log("✅ Package structure created successfully!");
            EditorUtility.DisplayDialog("Success", "Package structure created successfully!", "OK");
        }

        private void GeneratePackageFiles()
        {
            string packagePath = Path.Combine("Packages", packageName);
            
            // package.json
            string packageJson = $@"{{
  ""name"": ""{packageName}"",
  ""version"": ""{packageVersion}"",
  ""displayName"": ""{packageDisplayName}"",
  ""description"": ""Complete Unity project framework with Scene Management, UI System, Object Pooling, and Core Architecture patterns for rapid game development."",
  ""unity"": ""2021.3"",
  ""documentationUrl"": ""{repositoryUrl}/wiki"",
  ""changelogUrl"": ""{repositoryUrl}/blob/main/CHANGELOG.md"",
  ""licensesUrl"": ""{repositoryUrl}/blob/main/LICENSE.md"",
  ""dependencies"": {{
    ""com.unity.textmeshpro"": ""3.0.6""
  }},
  ""keywords"": [
    ""framework"",
    ""architecture"",
    ""scene-management"",
    ""ui-system"",
    ""object-pooling"",
    ""singleton"",
    ""core-systems""
  ],
  ""author"": {{
    ""name"": ""{authorName}"",
    ""email"": ""{authorEmail}"",
    ""url"": ""{repositoryUrl}""
  }},
  ""type"": ""library"",
  ""samples"": [
    {{
      ""displayName"": ""Basic Setup Example"",
      ""description"": ""Basic scene setup with Core Framework components"",
      ""path"": ""Samples~/BasicSetup""
    }},
    {{
      ""displayName"": ""Complete Framework Example"",
      ""description"": ""Complete example showing all framework features"",
      ""path"": ""Samples~/CompleteExample""
    }},
    {{
      ""displayName"": ""Performance Benchmarks"",
      ""description"": ""Performance testing tools and benchmarks"",
      ""path"": ""Samples~/Benchmarks""
    }}
  ]
}}";
            
            File.WriteAllText(Path.Combine(packagePath, "package.json"), packageJson);
            
            // README.md
            string readme = $@"# {packageDisplayName}

A complete Unity project framework providing essential systems for rapid game development.

## 🚀 Features

### Core Architecture
- **Singleton Pattern**: Thread-safe singleton implementation for managers
- **Core System**: Centralized access to all framework managers
- **Bootstrap System**: Automatic initialization and dependency management

### Scene Management
- **Advanced Scene Loading**: Async loading with progress tracking
- **Scene Transitions**: Customizable fade effects and loading screens
- **Scene Events**: Comprehensive event system for scene lifecycle
- **Preloading**: Background scene loading for instant transitions

### UI System
- **Modular UI Architecture**: Separated concerns (Popup, Toast, Effects, Input)
- **Popup Management**: Stack-based popup system with automatic handling
- **Toast System**: Queue-based notification system
- **UI Effects**: Fade transitions and visual effects

### Object Pooling
- **Generic Object Pool**: Type-safe object pooling system
- **Prefab Pools**: Specialized pools for GameObject prefabs
- **Performance Monitoring**: Real-time pool statistics and optimization

## 📦 Installation

### Package Manager
1. Open Package Manager in Unity
2. Click ""+"" → ""Add package from git URL""
3. Enter: `{repositoryUrl}.git`

### Local Installation
1. Download this repository
2. Copy to your project's `Packages` directory
3. Unity will automatically import the package

## 🛠️ Quick Start

### Basic Setup
```csharp
// Access core systems anywhere in your code
var ui = Core.app.ui;
var sceneManager = AdvancedSceneManager.Instance;
var poolManager = ObjectPoolManager.Instance;

// Show a toast message
ui.ShowToast(""Welcome to the game!"");

// Load a scene with transition
sceneManager.LoadScene(""GameScene"");

// Get an object from pool
GameObject bullet = poolManager.Get(bulletPrefab);
```

### Scene Management
```csharp
// Basic scene loading
AdvancedSceneManager.Instance.LoadScene(""GameScene"");

// Custom transition
var transition = new SceneTransitionSettings {{
    useFadeTransition = true,
    fadeColor = Color.black,
    fadeOutDuration = 1.0f,
    fadeInDuration = 0.5f
}};
AdvancedSceneManager.Instance.LoadScene(""GameScene"", transition);
```

### UI System
```csharp
// Show popup
Core.app.ui.ShowSystemPopup(""Are you sure?"", ""Yes"", ConfirmAction, ""No"", CancelAction);

// Show toast
Core.app.ui.ShowToast(""Item collected!"");
```

### Object Pooling
```csharp
// Get from pool
GameObject obj = ObjectPoolManager.Instance.Get(prefab);

// Return to pool
ObjectPoolManager.Instance.Return(obj);
```

## 📚 Documentation

For detailed documentation, examples, and API reference, visit our [Wiki]({repositoryUrl}/wiki).

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.md) file for details.

## 💬 Support

- **Issues**: Report bugs and feature requests on [GitHub]({repositoryUrl}/issues)
- **Documentation**: Check the [wiki]({repositoryUrl}/wiki) for detailed guides

---

**Made with ❤️ by {authorName}**
";
            
            File.WriteAllText(Path.Combine(packagePath, "README.md"), readme);
            
            // CHANGELOG.md
            string changelog = $@"# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [{packageVersion}] - {System.DateTime.Now.ToString("yyyy-MM-dd")}

### Added
- Initial release of {packageDisplayName}
- Core Framework with centralized manager access
- Advanced Scene Management system with transitions and preloading
- Modular UI System with popups, toasts, and effects
- High-performance Object Pooling system
- Comprehensive documentation and examples
- Editor tools for easy setup and configuration

### Features
- Thread-safe Singleton pattern implementation
- Async scene loading with progress tracking
- Customizable scene transitions and effects
- Stack-based popup management system
- Queue-based toast notification system
- Generic and prefab-specific object pools
- Performance monitoring and benchmarking tools
- Automatic initialization and dependency management

### Documentation
- Complete API documentation
- Usage examples and tutorials
- Performance benchmarks and best practices
- Migration guides and troubleshooting
";
            
            File.WriteAllText(Path.Combine(packagePath, "CHANGELOG.md"), changelog);
            
            // LICENSE.md
            string license = $@"MIT License

Copyright (c) {System.DateTime.Now.Year} {authorName}

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
";
            
            File.WriteAllText(Path.Combine(packagePath, "LICENSE.md"), license);
            
            AssetDatabase.Refresh();
            Debug.Log("✅ Package files generated successfully!");
            EditorUtility.DisplayDialog("Success", "Package files generated successfully!", "OK");
        }

        private void MoveScriptsToPackage()
        {
            string packagePath = Path.Combine("Packages", packageName);
            string sourcePath = "Assets/5_Scripts";
            
            // 스크립트 이동 매핑
            var moveMapping = new Dictionary<string, string>
            {
                { Path.Combine(sourcePath, "Core"), Path.Combine(packagePath, "Runtime", "Core") },
                { Path.Combine(sourcePath, "SceneManagement"), Path.Combine(packagePath, "Runtime", "SceneManagement") },
                { Path.Combine(sourcePath, "UI"), Path.Combine(packagePath, "Runtime", "UI") },
                { Path.Combine(sourcePath, "Module"), Path.Combine(packagePath, "Runtime", "Module") },
                { Path.Combine(sourcePath, "Editor"), Path.Combine(packagePath, "Editor", "Tools") },
                { Path.Combine(sourcePath, "Examples"), Path.Combine(packagePath, "Samples~", "CompleteExample") }
            };
            
            foreach (var mapping in moveMapping)
            {
                if (Directory.Exists(mapping.Key))
                {
                    CopyDirectory(mapping.Key, mapping.Value);
                    Debug.Log($"Copied: {mapping.Key} -> {mapping.Value}");
                }
            }
            
            AssetDatabase.Refresh();
            Debug.Log("✅ Scripts moved to package successfully!");
            EditorUtility.DisplayDialog("Success", "Scripts moved to package successfully!\n\nYou can now remove the original Assets/5_Scripts folder.", "OK");
        }

        private void CreateAssemblyDefinitions()
        {
            string packagePath = Path.Combine("Packages", packageName);
            
            // Runtime Assembly Definition
            string runtimeAsmdef = $@"{{
    ""name"": ""RedMinS.Runtime"",
    ""rootNamespace"": ""RedMinS"",
    ""references"": [
        ""Unity.TextMeshPro""
    ],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [
        {{
            ""name"": ""com.unity.textmeshpro"",
            ""expression"": ""3.0.0"",
            ""define"": ""TMP_PRESENT""
        }}
    ],
    ""noEngineReferences"": false
}}";
            
            File.WriteAllText(Path.Combine(packagePath, "Runtime", "RedMinS.Runtime.asmdef"), runtimeAsmdef);
            
            // Editor Assembly Definition
            string editorAsmdef = $@"{{
    ""name"": ""RedMinS.Editor"",
    ""rootNamespace"": ""RedMinS.Editor"",
    ""references"": [
        ""RedMinS.Runtime""
    ],
    ""includePlatforms"": [
        ""Editor""
    ],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}}";
            
            File.WriteAllText(Path.Combine(packagePath, "Editor", "RedMinS.Editor.asmdef"), editorAsmdef);
            
            AssetDatabase.Refresh();
            Debug.Log("✅ Assembly definitions created successfully!");
            EditorUtility.DisplayDialog("Success", "Assembly definitions created successfully!", "OK");
        }

        private void SetupGitRepository()
        {
            string instructions = @"Git Repository Setup Instructions:

1. Initialize Git repository:
   git init

2. Add files:
   git add .

3. Initial commit:
   git commit -m ""feat: Initial RedMinS Core Framework v1.0.0""

4. Add remote repository:
   git remote add origin YOUR_REPOSITORY_URL

5. Push to GitHub:
   git push -u origin main

6. Create version tag:
   git tag v1.0.0
   git push origin v1.0.0

7. Package Manager Installation:
   Add package from git URL: YOUR_REPOSITORY_URL.git

For detailed instructions, see the Documentation folder.";
            
            Debug.Log(instructions);
            EditorUtility.DisplayDialog("Git Setup Instructions", instructions, "OK");
        }

        private void CompleteMigration()
        {
            Debug.Log("🚀 Starting complete migration...");
            
            CreatePackageStructure();
            GeneratePackageFiles();
            MoveScriptsToPackage();
            CreateAssemblyDefinitions();
            
            Debug.Log("🎉 Migration completed successfully!");
            EditorUtility.DisplayDialog("Migration Complete", 
                "Package migration completed successfully!\n\n" +
                "Next steps:\n" +
                "1. Test the package in a new project\n" +
                "2. Setup Git repository\n" +
                "3. Push to GitHub\n" +
                "4. Install via Package Manager", "OK");
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string targetFile = Path.Combine(targetDir, fileName);
                File.Copy(file, targetFile, true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(directory);
                string targetSubDir = Path.Combine(targetDir, dirName);
                CopyDirectory(directory, targetSubDir);
            }
        }
    }
}