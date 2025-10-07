// ========================================
// Assembly Definition 파일들
// ========================================

/*
=== Runtime/RedMinS.Runtime.asmdef ===
{
    "name": "RedMinS.Runtime",
    "rootNamespace": "RedMinS",
    "references": [
        "Unity.TextMeshPro"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [
        {
            "name": "com.unity.textmeshpro",
            "expression": "3.0.0",
            "define": "TMP_PRESENT"
        }
    ],
    "noEngineReferences": false
}

=== Editor/RedMinS.Editor.asmdef ===
{
    "name": "RedMinS.Editor",
    "rootNamespace": "RedMinS.Editor",
    "references": [
        "RedMinS.Runtime"
    ],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}

=== Tests/Runtime/RedMinS.Tests.Runtime.asmdef ===
{
    "name": "RedMinS.Tests.Runtime",
    "rootNamespace": "RedMinS.Tests",
    "references": [
        "UnityEngine.TestRunner",
        "UnityEditor.TestRunner",
        "RedMinS.Runtime"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": true,
    "precompiledReferences": [
        "nunit.framework.dll"
    ],
    "autoReferenced": false,
    "defineConstraints": [
        "UNITY_INCLUDE_TESTS"
    ],
    "versionDefines": [],
    "noEngineReferences": false
}

=== Tests/Editor/RedMinS.Tests.Editor.asmdef ===
{
    "name": "RedMinS.Tests.Editor",
    "rootNamespace": "RedMinS.Tests.Editor",
    "references": [
        "UnityEngine.TestRunner",
        "UnityEditor.TestRunner", 
        "RedMinS.Runtime",
        "RedMinS.Editor"
    ],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": true,
    "precompiledReferences": [
        "nunit.framework.dll"
    ],
    "autoReferenced": false,
    "defineConstraints": [
        "UNITY_INCLUDE_TESTS"
    ],
    "versionDefines": [],
    "noEngineReferences": false
}
*/