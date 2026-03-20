# Unity 프로젝트 코드 작성 규칙

## 네임스페이스

| 용도 | 네임스페이스 |
|---|---|
| 기본 게임 로직 | `RedMinS` |
| UI 전용 컴포넌트 | `RedMinS.UI` |
| 네이티브 기능 | `RedMinS.Native` |

---

## 클래스 명명 규칙

모든 클래스명은 **PascalCase** 기본. 역할에 따라 아래 접두/접미어를 붙인다.

### 접미어 규칙

| 접미어 | 용도 | 예시 |
|---|---|---|
| `Manager` | 씬 전체를 관리하는 싱글톤성 매니저 | `GameManager`, `UIManager`, `SoundManager` |
| `System` | 특정 게임 기능 단위의 로직 처리 클래스 | `QuestSystem`, `EpisodeSystem`, `WalkOutsideSystem` |
| `Data` | 런타임에서 사용하는 데이터 모델 | `UserData`, `PuppyboyData`, `HomeDecorData` |
| `DataForDB` | Firebase 등 DB에 직접 저장/로드하는 데이터 구조 | `UserDataForDB`, `EpisodeDataForDB` |
| `Info` | 테이블(CSV/JSON)에서 읽어오는 정적 정보 | `ItemInfo`, `QuestInfo`, `PuppyboyInfo` |
| `Container` | 여러 리소스를 모아 제공하는 클래스 | `TableContainer`, `AssetContainer`, `SpriteContainer` |
| `Loader` | 데이터를 로드하는 전용 클래스 | `UserDatabaseLoader`, `AssetLoader` |

### UI 클래스 접두어

모든 UI MonoBehaviour 클래스는 `UI` 접두어를 붙인다.

| 패턴 | 용도 | 예시 |
|---|---|---|
| `UI` + 이름 | 일반 UI 컴포넌트 | `UITabMenu`, `UIGaugeBar`, `UIBadge` |
| `UI` + 이름 + `Popup` | 팝업 | `UIShopPopup`, `UIGachaPopup`, `UISystemPopup` |
| `UI` + 이름 + `Slot` | 리스트/그리드의 아이템 슬롯 | `UIItemSlot`, `UIQuestSlot`, `UIUpgradeableUnitSlot` |
| `UI` + 이름 + `Page` | 탭/페이지 단위 화면 | `UIShopItemPage`, `UIAlbumEpisodePage` |
| `UI` + 이름 + `Menu` | 슬라이드/탭 메뉴 | `UIIdleMenu`, `UIClosetMenu`, `UIDialogMenu` |

### 게임 오브젝트 씬 컨트롤러

씬 단위의 메인 컨트롤러는 접두/접미어 없이 역할 이름만 사용한다.

```
PuppyboyHome, PuppyboyRoom, OutsideManager
```

---

## 필드 및 프로퍼티 명명 규칙

### private 필드

```csharp
// [SerializeField] 인스펙터 노출 필드 → camelCase (언더스코어 없음)
[SerializeField] UITabMenu tabMenu;
[SerializeField] UIUpgradeableUnitSlot[] homeDecorSlots;
[SerializeField] TextMeshProUGUI txtName;

// 코드에서만 사용하는 private 필드 → _camelCase
UserData _user;
TableContainer _table;
List<UIPopup> _onPopups;
```

### public 프로퍼티

```csharp
// 외부에서 읽기만 허용 (private set) → camelCase
public string userId { private set; get; }
public bool isReady { private set; get; }

// 외부에서 자유롭게 접근하는 복합 데이터 → PascalCase
public UserInventoryData Inventory { private set; get; }
public UserEpisodeData Episode { private set; get; }
```

### 상수

```csharp
// 상수 → SCREAMING_SNAKE_CASE
public const int STUDIO_MODE_OPEN_ITEM = 6301;
public int RPS_SCORE_FOR_FIRST_WIN { get { return rpsScoreForFirstWin; } }
```

---

## UI 컴포넌트 필드 접두어

SerializeField로 인스펙터에서 할당하는 Unity UI 컴포넌트는 타입 접두어를 사용한다.

| 접두어 | 타입 | 예시 |
|---|---|---|
| `img` | `Image` | `imgSlotBg`, `imgIcon` |
| `txt` | `TextMeshProUGUI` / `Text` | `txtName`, `txtLv`, `txtUpgradePrice` |
| `btn` | `Button` | `btnUpgrade` |
| `Ani` (접미어) | `Animator` / `Animation` | `fadeAni` |

---

## 메서드 명명 규칙

### 일반 메서드 → PascalCase

```csharp
void SetMenu() { }
void DisplaySlot() { }
bool CanReceiveQuestReward(QuestInfo quest) { }
```

### 버튼 클릭 핸들러 → `Click` + PascalCase

인스펙터의 OnClick 이벤트나 AddListener로 연결되는 메서드.

```csharp
void ClickUpgrade() { }
void ClickMenuUp() { }
public void ClickItemGacha() { }  // 인스펙터 연결 시 public
```

### 코루틴 메서드 → `IE` + PascalCase

IEnumerator 반환 메서드는 반드시 `IE` 접두어를 붙인다.

```csharp
IEnumerator IEFadeIn() { }
IEnumerator IEFadeOut(Color screenColor) { }
IEnumerator IEToast() { }
```

### 이벤트 → `On` + PascalCase

```csharp
public event UnityAction<int> OnAchieveQuest;
public event UnityAction OnEarnMoney;
public static event UnityAction<int> OnAdopt;
```

---

## GameManager 시스템 필드 접미어

`GameManager` 내에서 각종 `System` 인스턴스를 보관할 때는 `Sym` 접미어를 사용한다.

```csharp
public EpisodeSystem episodeSym { private set; get; }
public QuestSystem questSym { private set; get; }
public IdleSystem idleSym { private set; get; }
public IdleSkillSystem skillSym { private set; get; }
```

---

## 전역 접근 패턴

`Core` 정적 클래스를 통해 매니저에 접근한다. `FindObjectOfType` 직접 호출을 지양한다.

```csharp
// 전역 매니저 접근
Core.app.ui          // UIManager
Core.app.game        // GameManager
Core.app.table       // TableContainer
Core.app.sound       // SoundManager
Core.app.asset       // AssetContainer
Core.app.scene       // SceneLoader

Core.network         // NetworkManager
Core.database        // DatabaseManager
Core.native          // NativeManager

// UserData는 Singleton으로 직접 접근
UserData.Instance
```

---

## 인스펙터 헤더 규칙

SerializeField 그룹을 구분할 때 `[Header("- 이름")]` 형식을 사용한다.

```csharp
[Header("- tab Menu")]
[SerializeField] UITabMenu tabMenu;

[Header("- home decor")]
[SerializeField] UIUpgradeableUnitSlot[] homeDecorSlots;
```

---

## DB 데이터 인터페이스

DB에 저장/로드하는 모든 데이터 클래스는 `DataForDB` 인터페이스를 구현한다.

```csharp
public interface DataForDB
{
    Dictionary<string, object> ToDictionary();
}
```

DB 키는 해당 클래스 내에 `KEY_` 접두어 상수로 정의한다.

```csharp
public static readonly string KEY_coin = "coin";
public static readonly string KEY_nickname = "nickname";
```

---

## 테이블 정보 클래스

정적 게임 데이터는 `TableInfo`를 상속한다.

```csharp
public class ItemInfo : TableInfo { ... }
public class QuestInfo : TableInfo { ... }
```

- `TableInfo`는 `uid` 필드를 공통으로 가진다 (프로퍼티가 아닌 public 필드로 선언).
- 리소스(스프라이트, 문자열) 조회는 Info 클래스 내 메서드로 캡슐화한다.

```csharp
public string Name() { ... }
public Sprite Icon() { ... }
```

---

## 폴더 구조 규칙

```
Assets/
  0_Scenes/          # Unity 씬 파일
  1_Graphics/        # 이미지, 스파인 등 그래픽 리소스
  2_Prefabs/         # 프리팹
  3_DataTables/      # CSV / JSON 데이터 테이블
  4_Sounds/          # 오디오 클립
  5_Scripts/
    Core/
      Manager/       # 전역 매니저 (GameManager, UIManager 등)
      Module/        # 재사용 가능한 독립 모듈 (ObjectPool, SwipeSensor 등)
      Game/
        _PBHOME/     # 씬별 스크립트 (언더스코어 + 씬명 대문자)
        _PBROOM/
        _OUTSIDE/
        GameUI/      # 씬 공통 게임 UI
        System/      # 게임 시스템 로직
        Table/       # TableInfo 클래스들
        UserData/    # 유저 데이터 모델
      UI/            # 공통 UI 컴포넌트
```

씬 전용 스크립트 폴더는 `_씬명` 형식(언더스코어 + 대문자)으로 네이밍한다.

---

## 기타 규칙

- **Singleton MonoBehaviour**: `SingletonMonobehaviour<T>` 베이스 클래스를 상속한다.
- **static 캐싱**: 슬롯처럼 다수 인스턴스가 생성되는 UI 클래스에서 공통으로 쓰는 매니저 참조는 `static`으로 캐싱한다.
  ```csharp
  static UserData _user = null;
  static StringTable _uiString = null;
  ```
- **오브젝트 풀**: 반복 생성/삭제되는 UI(팝업, 토스트, 슬롯)는 `ObjectPool`을 통해 관리한다.
- **코루틴 관리**: `CoroutineOperator` 헬퍼를 사용해 코루틴을 외부에서 실행한다.
- **다국어**: 하드코딩 문자열 대신 `StringTable.GetString(uid)`를 사용한다.
