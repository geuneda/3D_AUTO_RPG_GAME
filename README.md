# Unity RPG Game Project

## 개발기간
- 2024-11-12 ~ 2024-11-13 (2일)

## 개요
자동전투 RPG 게임으로, 스테이지 기반의 진행 시스템을 갖추고 있습니다.

## 주요 기능

### 스테이지 시스템
- 스테이지별 난이도 증가
- 보스 처치 시 다음 스테이지로 자동 진행
- 스테이지 선택 UI를 통한 난이도 조절 (TAP 키로 가능)
- 스테이지별 몬스터 강화 및 보상 증가

### 전투 시스템
- 자동 전투
- 적 탐지 및 추적
- 전투/비전투 상태에 따른 카메라 전환

### 맵 시스템
- 절차적 맵 생성
- NavMesh를 통한 경로 탐색
- 메시 결합을 통한 최적화

### 이벤트 시스템
- GameEventManager를 통한 이벤트 관리

## UI 시스템

### 로딩 UI
- 스테이지 정보 표시
- 진행 상태 표시
- 랜덤 팁 메시지

### 스테이지 선택 UI
- 스테이지별 난이도 정보
- 보상 증가율 표시
- 스테이지 선택 기능

## 카메라 시스템
- Cinemachine 활용
- 전투/비전투 상태에 따른 카메라 전환
- 플레이어 추적 및 전투 연출

## 중요 개발 규칙
1. 모든 이벤트는 `GameEventManager`를 통해 관리
2. 싱글톤 패턴은 `Singleton<T>` 베이스 클래스 활용

## Known Issues (알려진 버그)

### UI 관련
- [ ] 스테이지가 바뀌고 난 후 UI가 업데이트 되지 않는 문제

### 전투 관련
- [ ] 전투 중 카메라 전환이 부자연스러운 현상
- [ ] 스테이지가 바뀌고 난 후 몬스터를 초기화 하지 않아 생기는 다양한 문제
- [ ] 몬스터가 죽고 난 자리에 플레이어가 있으면 간헐적으로 데미지를 받는 현상이 있음 Unity RPG Game Project

## 개발기간
- 2024-11-12 ~ 2024-11-13 (2일)

## 개요
자동전투 RPG 게임으로, 스테이지 기반의 진행 시스템을 갖추고 있습니다.

## 주요 기능

### 스테이지 시스템
- 스테이지별 난이도 증가
- 보스 처치 시 다음 스테이지로 자동 진행
- 스테이지 선택 UI를 통한 난이도 조절 (TAP 키로 가능)
- 스테이지별 몬스터 강화 및 보상 증가

### 전투 시스템
- 자동 전투
- 적 탐지 및 추적
- 전투/비전투 상태에 따른 카메라 전환

### 맵 시스템
- 절차적 맵 생성
- NavMesh를 통한 경로 탐색
- 메시 결합을 통한 최적화

### 이벤트 시스템
- GameEventManager를 통한 이벤트 관리

## UI 시스템

### 로딩 UI
- 스테이지 정보 표시
- 진행 상태 표시
- 랜덤 팁 메시지

### 스테이지 선택 UI
- 스테이지별 난이도 정보
- 보상 증가율 표시
- 스테이지 선택 기능

## 카메라 시스템
- Cinemachine 활용
- 전투/비전투 상태에 따른 카메라 전환
- 플레이어 추적 및 전투 연출

## 중요 개발 규칙
1. 모든 이벤트는 `GameEventManager`를 통해 관리
2. 싱글톤 패턴은 `Singleton<T>` 베이스 클래스 활용

## Known Issues (알려진 버그)

### UI 관련
- [ ] 스테이지가 바뀌고 난 후 UI가 업데이트 되지 않는 문제

### 전투 관련
- [ ] 전투 중 카메라 전환이 부자연스러운 현상
- [ ] 스테이지가 바뀌고 난 후 몬스터를 초기화 하지 않아 생기는 다양한 문제
- [ ] 몬스터가 죽고 난 자리에 플레이어가 있으면 간헐적으로 데미지를 받는 현상이 있음
