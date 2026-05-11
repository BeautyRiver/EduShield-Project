# EduShield (Safety Hero) - 설계 문서

## 프로젝트 개요

- **엔진**: Unity 2D (URP)
- **장르**: Vampire Survivors 스타일 탑다운 슈터 + **변신 시스템** 추가
- **특징**: 아트 담당 없음 → 전량 에셋 사용
- **교육 테마**: 미정 (게임플레이 방향성 우선 확정)

---

## 현재 구현 상태

### 씬 구성
| 씬 | 설명 |
|----|------|
| Title Scene | 타이틀 |
| Login | Firebase 로그인 |
| Loading Scene | 로딩 |
| Lobby Scene | 자유 이동 + NPC 상호작용 |
| Game Scene | 인게임 |
| Intro Scene | 인트로 |

### 핵심 스크립트 구조 (`Assets/Safety Hero/Scripts/`)

```
Manager/
  GameManager.cs       - 게임 진행, 레벨업, 승리/패배
  GlobalManager.cs     - GameState/PlayerState enum, 액션맵 전환
  HUDManager.cs        - 체력/경험치/킬수/골드/시간 UI
  SpawnManager.cs      - LevelDesign 기반 웨이브 스폰
  WeaponManager.cs     - 무기 추가/강화, SetAttackState(bool)
  UIManager.cs
  EquipmentManager.cs
  PoolManager.cs
  AudioManager.cs

Player/
  PlayerInGame.cs      - 체력, 경험치, 레벨업, RecalculateStats
  PlayerMove.cs        - 이동 (FixedUpdate, FreeMove 상태 체크)
  PlayerInputController.cs - Input System 콜백 허브
  PlayerInputController.cs   OnMove / OnInteract / OnPause

Data/
  PlayerData.cs        - characterId, maxHpMult, damageMult, attackSpeedMult, attackRangeMult, speedMult, animCon
  EnemyData.cs
  StageData.cs
  1.ItemDatas/         - BulletData, GearData, EtcData, ItemData

Objects (Items)/
  Weapons/Weapon.cs    - abstract base, RecalculateStats() (gm.playerData 배율 적용)
  Weapons/W0~W52       - 구체 무기 구현
  Gears/Gear.cs
```

### 입력 시스템 (`Assets/Safety Hero/InputSystem_Actions.inputactions`)
| 액션맵 | 등록된 액션 |
|--------|------------|
| InGame | Move(WASD/화살표), Pause(ESC), Interact(E), Crouch(C - 미사용), Sprint(Shift - 미사용) |
| InLobby | Move, Interact(E) |
| UI | Navigate, Submit, Cancel, Pause 등 |
| Empty | 없음 (비활성화용) |

### GameState / PlayerState
```csharp
enum GameState  { Lobby, Ready, Playing, Paused, LevelUp, GameOver, Victory }
enum PlayerState { FreeMove, InUI }
```

---

## 신규 설계: 변신 시스템

### 컨셉
바우트 게임처럼 **변신 게이지**를 모아 **특수 변신**을 발동하는 시스템.  
Vampire Survivors 루프에 "플레이어 스킬 표현 타이밍"을 추가해 게임성 차별화.

### 게이지 충전 3가지
| 방법 | 수치 (기본값) |
|------|--------------|
| 시간 자동 누적 | 초당 1.5 |
| 적 처치 | 처치당 8 |
| 기 모으기 (버튼 홀드) | 초당 25 |

### 기 모으기 패널티
- **이동 완전 정지** + **공격 불가**
- 적이 몰릴 때 안전한 타이밍을 노려야 하는 전략적 리스크

### 변신 발동
- 게이지 MAX 상태에서 별도 버튼 → **즉시 변신**
- 변신 중: 스탯 강화 + 스프라이트/애니메이터 교체
- 변신 종료: 게이지 0 초기화 → Normal 상태로 복귀
- HUD: 변신 중에는 게이지가 남은 시간 표시로 전환

### TransformState (신규 enum)
```
Normal     → 일반 플레이 (시간 자동 충전)
Charging   → 기 모으기 중 (이동/공격 불가, 빠른 충전)
Transformed → 변신 완료 (강화 상태, 타이머 카운트다운)
```

### 변신 폼 (TransformData)
- ScriptableObject로 관리
- 필드: formName, icon, description, animCon, durationSeconds, damageMult, speedMult, attackSpeedMult, attackRangeMult, price, isDefault
- **상점에서 구매**하는 방식 → 메타 수집 요소
- 변신 시 animCon 교체 → 다른 스프라이트 적용
- 복귀 시 `GameManager.instance.playerData.animCon`으로 원래 캐릭터 복원

---

## 구현 계획 (미작업)

### 새로 만들 파일
| 파일 | 위치 | 설명 |
|------|------|------|
| `TransformData.cs` | `Scripts/Player/` | 변신 폼 ScriptableObject |
| `TransformationSystem.cs` | `Scripts/Player/` | 게이지/변신 상태 관리 컴포넌트 (Player에 부착) |

### 수정할 파일
| 파일 | 수정 내용 |
|------|----------|
| `InputSystem_Actions.inputactions` | InGame 맵에 **Charge**(Value, Space/RB) + **Transform**(Button, F/X버튼) 액션 추가 |
| `PlayerInputController.cs` | `OnCharge(InputValue)`, `OnTransform(InputValue)` 콜백 추가 |
| `PlayerMove.cs` | `FixedUpdate`에서 `TransformationSystem.instance.IsCharging` 시 이동 차단 |
| `PlayerInGame.cs` | `IncreasePlayerKill()`에서 `AddGaugeFromKill()` 호출, `RecalculateStats()`에 speed 변신 배율 적용 |
| `Weapon.cs` (base) | `RecalculateStats()`에 TransformationSystem damage/attackSpeed/attackRange 배율 곱하기 |
| `HUDManager.cs` | `UpdateTransformGauge(float cur, float max)` 메서드 + Slider 필드 추가 |
| `PlayerData.cs` | `TransformData equippedTransform` 필드 추가 |

### TransformationSystem 핵심 로직
```csharp
// 기 모으기 시작 - 무기 비활성화
public void StartCharging() {
    state = TransformState.Charging;
    weaponManager?.SetAttackState(false); // WeaponManager.SetAttackState 이미 구현됨
}

// 기 모으기 해제
public void StopCharging() {
    state = TransformState.Normal;
    weaponManager?.SetAttackState(true);
}

// 변신 발동 (게이지 MAX 필요)
public void TryTransform() {
    state = TransformState.Transformed;
    anim.runtimeAnimatorController = equippedForm.animCon;
    player.RecalculateStats();
    RefreshWeaponStats(); // 모든 무기 RecalculateStats 호출
}

// 변신 해제 (타이머 종료)
private void RevertTransform() {
    state = TransformState.Normal;
    currentGauge = 0f;
    anim.runtimeAnimatorController = GameManager.instance.playerData.animCon;
    player.RecalculateStats();
    RefreshWeaponStats();
}

// 배율 반환 (Weapon/PlayerInGame에서 사용)
public float GetDamageBonus()      => IsTransformed ? equippedForm.damageMult : 1f;
public float GetSpeedBonus()       => IsTransformed ? equippedForm.speedMult : 1f;
public float GetAttackSpeedBonus() => IsTransformed ? equippedForm.attackSpeedMult : 1f;
public float GetAttackRangeBonus() => IsTransformed ? equippedForm.attackRangeMult : 1f;
```

### WeaponManager.SetAttackState (이미 구현됨)
```csharp
public void SetAttackState(bool canAttack) {
    foreach (var weapon in activeWeapons) {
        weapon.isAttacking = !canAttack;
        weapon.enabled = canAttack;
    }
}
```

### Weapon.RecalculateStats 수정 포인트
```csharp
// 기존 코드에 TransformationSystem 배율 추가
var ts = TransformationSystem.instance;
finalStats.damage = currentBulletData.baseDamage * pData.damageMult * (ts?.GetDamageBonus() ?? 1f);
finalStats.weaponAttackSpeed = currentBulletData.baseWeaponAttackSpeed / pData.attackSpeedMult / (ts?.GetAttackSpeedBonus() ?? 1f);
finalStats.attackRange = currentBulletData.baseRange * pData.attackRangeMult * (ts?.GetAttackRangeBonus() ?? 1f);
```

### PlayerMove.cs 수정 포인트
```csharp
private void FixedUpdate() {
    if (GlobalManager.instance.playerState != PlayerState.FreeMove) return;
    if (TransformationSystem.instance != null && TransformationSystem.instance.IsCharging) return; // 추가
    Move();
}
```

### PlayerInGame.RecalculateStats 수정 포인트
```csharp
float speedBonus = TransformationSystem.instance?.GetSpeedBonus() ?? 1f;
playerMove.SetCurretSpeed(playerMove.baseSpeed * GameManager.instance.playerData.speedMult * speedBonus);
```

---

## 주의사항

- `TransformationSystem`은 Player 루트 GameObject에 컴포넌트로 추가 필요
- `WeaponManager`는 Player의 `weaponObject` 자식에 있음 → `GetComponentInChildren<WeaponManager>()` 로 참조
- animCon 복원 시 `originalAnimCon` 캐싱 대신 `GameManager.instance.playerData.animCon` 사용 (PlayerInit 실행 순서 문제 방지)
- 변신 HUD 게이지: Normal/Charging 상태 → 충전량 표시, Transformed 상태 → 남은 시간 표시
- `TransformationSystem.instance`는 싱글톤, Weapon.cs에서 null 체크 필수 (`??` 연산자 활용)
