/*
 * ============================================================================
 * Firebase 설정 가이드 (RedTemplate)
 * ============================================================================
 *
 * 이 파일은 실행용 코드가 아닌 설정 가이드 문서입니다.
 *
 * ============================================================================
 * 1. Firebase 프로젝트 생성
 * ============================================================================
 *
 * 1) Firebase Console (https://console.firebase.google.com) 에서 프로젝트 생성
 * 2) Android/iOS 앱 등록
 * 3) 설정 파일 다운로드:
 *    - Android: google-services.json → Assets/StreamingAssets/
 *    - iOS: GoogleService-Info.plist → Assets/
 *
 *
 * ============================================================================
 * 2. Firebase Unity SDK 설치
 * ============================================================================
 *
 * 방법 A: .unitypackage (권장)
 *   1) Firebase Unity SDK 다운로드: https://firebase.google.com/download/unity
 *   2) 필요한 패키지만 Import:
 *      - FirebaseAuth.unitypackage
 *      - FirebaseDatabase.unitypackage
 *      - (선택) FirebaseAnalytics.unitypackage
 *   3) External Dependency Manager가 자동으로 네이티브 종속성 해결
 *
 * 방법 B: UPM (Unity Package Manager)
 *   Packages/manifest.json에 추가:
 *   "com.google.firebase.auth": "https://...",
 *   "com.google.firebase.database": "https://..."
 *
 *
 * ============================================================================
 * 3. Scripting Define Symbols 설정
 * ============================================================================
 *
 * Project Settings → Player → Scripting Define Symbols 에 추가:
 *
 *   FIREBASE_AUTH;FIREBASE_DATABASE
 *
 * 이 심볼이 없으면 Firebase 관련 코드가 컴파일에서 제외됩니다.
 * Firebase SDK 설치 전에는 추가하지 마세요.
 *
 *
 * ============================================================================
 * 4. 초기화 코드 예시
 * ============================================================================
 *
 * --- 방법 1: Core.InitializeWithFirebase (간편) ---
 *
 *   void Start()
 *   {
 *       // 인증 제공자 등록
 *       Core.auth.RegisterProvider(new AnonymousAuthProvider());
 *       Core.auth.RegisterProvider(new GoogleAuthProvider("your-web-client-id"));
 *
 *       // 카카오 로그인 (Custom Token 방식)
 *       Core.auth.RegisterProvider(new CustomTokenAuthProvider(
 *           "Kakao",
 *           "https://your-region-your-project.cloudfunctions.net/createCustomToken"
 *       ));
 *
 *       // Firebase 초기화 + 익명 로그인 + DB 연결
 *       Core.InitializeWithFirebase("Anonymous",
 *           onComplete: () => Debug.Log("Ready!"),
 *           onFail: error => Debug.LogError(error)
 *       );
 *   }
 *
 *
 * --- 방법 2: 수동 초기화 (세밀한 제어) ---
 *
 *   void Start()
 *   {
 *       Core.Initialize();
 *
 *       Core.auth.InitializeFirebase(onReady: () =>
 *       {
 *           Core.auth.RegisterProvider(new AnonymousAuthProvider());
 *
 *           Core.auth.SignIn("Anonymous",
 *               onSuccess: uid =>
 *               {
 *                   Core.database.SetDataStore(new FirebaseDataStore(uid));
 *                   Core.network.StartConnectionMonitor();
 *                   LoadUserData();
 *               },
 *               onFail: error => ShowErrorPopup(error)
 *           );
 *       });
 *   }
 *
 *
 * ============================================================================
 * 5. 데이터 읽기/쓰기 예시
 * ============================================================================
 *
 *   // 비동기 저장 (Firebase 사용 시 권장)
 *   Core.database.SaveDataAsync("settings", settingsData,
 *       onComplete: () => Debug.Log("Saved!"),
 *       onError: error => Debug.LogError(error)
 *   );
 *
 *   // 비동기 로드
 *   Core.database.LoadDataAsync<UserSettings>("settings",
 *       onComplete: data => ApplySettings(data),
 *       onError: error => Debug.LogError(error)
 *   );
 *
 *   // FirebaseDataStore 직접 접근 (Dictionary 기반 고급 사용)
 *   var fbStore = Core.database.DataStore as FirebaseDataStore;
 *   if (fbStore != null)
 *   {
 *       fbStore.SetValue("currency/coin", 1000);
 *       fbStore.GetData("inventory", data => { ... });
 *       fbStore.GetGlobalData("AppConfig", config => { ... });
 *   }
 *
 *
 * ============================================================================
 * 6. 카카오/네이버 로그인 설정
 * ============================================================================
 *
 * Custom Token 방식으로 동작합니다. 서버(Cloud Function)가 필요합니다.
 * 자세한 내용은 CloudFunction_CustomAuth.js 파일을 참고하세요.
 *
 * 클라이언트 측:
 *   1) 카카오/네이버 SDK Unity 플러그인 설치
 *   2) SDK로 로그인하여 액세스 토큰 획득
 *   3) CustomTokenAuthProvider 사용:
 *
 *      var kakaoAuth = Core.auth.GetProvider<CustomTokenAuthProvider>("Kakao");
 *      StartCoroutine(kakaoAuth.SignInWithAccessToken(kakaoAccessToken,
 *          onSuccess: uid => Debug.Log($"Kakao login: {uid}"),
 *          onFail: error => Debug.LogError(error)
 *      ));
 *
 *
 * ============================================================================
 * 7. NetworkPolicy 설정
 * ============================================================================
 *
 * NetworkManager Inspector에서 Network Policy를 설정합니다:
 *
 *   - AlwaysRequired: 네트워크 끊기면 앱 종료 (재화 관리 게임용, 기본값)
 *   - RequiredForSync: 끊기면 쓰기 차단, 읽기는 캐시 허용
 *   - Optional: 오프라인 허용 (싱글플레이 게임용)
 *
 *
 * ============================================================================
 * 8. Firebase Realtime Database 보안 규칙 예시
 * ============================================================================
 *
 *   {
 *     "rules": {
 *       "Users": {
 *         "$uid": {
 *           ".read": "auth != null && auth.uid == $uid",
 *           ".write": "auth != null && auth.uid == $uid"
 *         }
 *       },
 *       "AppConfig": {
 *         ".read": "auth != null",
 *         ".write": false
 *       }
 *     }
 *   }
 *
 * ============================================================================
 */

// 이 파일은 컴파일되지 않습니다. 가이드 문서 용도입니다.
