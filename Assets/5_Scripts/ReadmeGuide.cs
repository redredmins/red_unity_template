// ========================================
// README.md 생성용 가이드 (README.md로 저장하세요)
// ========================================

/*
# RedMinS Core Framework

A complete Unity project framework providing essential systems for rapid game development.

## Features

### Core Architecture
- Singleton Pattern: Thread-safe singleton implementation for managers
- Core System: Centralized access to all framework managers
- Bootstrap System: Automatic initialization and dependency management

### Scene Management
- Advanced Scene Loading: Async loading with progress tracking
- Scene Transitions: Customizable fade effects and loading screens
- Scene Events: Comprehensive event system for scene lifecycle
- Preloading: Background scene loading for instant transitions
- Parallel Loading: Load multiple scenes simultaneously

### UI System
- Modular UI Architecture: Separated concerns (Popup, Toast, Effects, Input)
- Popup Management: Stack-based popup system with automatic handling
- Toast System: Queue-based notification system
- UI Effects: Fade transitions and visual effects
- Resolution Support: Automatic mobile and desktop resolution handling
- Input Management: Unified input handling with mobile optimization

### Object Pooling
- Generic Object Pool: Type-safe object pooling system
- Prefab Pools: Specialized pools for GameObject prefabs
- IPoolable Interface: Automatic lifecycle management for pooled objects
- Performance Monitoring: Real-time pool statistics and optimization
- Memory Management: Automatic cleanup and size limits

## Quick Start

```csharp
// Access core systems anywhere in your code
var ui = Core.app.ui;
var sceneManager = Core.app.scene;
var poolManager = ObjectPoolManager.Instance;

// Show a toast message
ui.ShowToast("Welcome to the game!");

// Load a scene with transition
sceneManager.LoadScene("GameScene");

// Get an object from pool
GameObject bullet = poolManager.Get(bulletPrefab);
```

## Installation

### Method 1: Package Manager (Recommended)
1. Open Package Manager in Unity
2. Click "+" → "Add package from git URL"
3. Enter your repository URL

### Method 2: Unity Asset Package
1. Export the framework as .unitypackage
2. Import in new projects via Assets → Import Package → Custom Package

## Architecture Overview

Core Framework
├── Core/                      // Core systems
├── SceneManagement/          // Scene loading and transitions
├── UI/                       // Modular UI system
├── Module/                   // Utilities and patterns
└── Examples/                 // Usage examples

## Best Practices

1. Initialize CoreBootstrap first
2. Use proper scene configuration
3. Implement IPoolable for complex pooled objects
4. Monitor performance with built-in tools

## License

MIT License - see LICENSE file for details.
*/