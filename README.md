<img width="640" height="360" alt="Image" src="https://github.com/user-attachments/assets/2193ee0b-6cc2-4960-8b8c-1adc29605d97" />

<br>
<br>

영상 : /www.youtube.com/watch?v=HgPu9-6DlWo

<br>

>- 장르 : 3D 공포 보드
>- 인원 : 1인
>- 엔진 : Unity
>- 플랫폼 : PC

### 요약

- 약간의 어드밴티지를 갖고 AI와 체스를 하는 공포 게임
- 라이트 베이킹, 모델링 포함 3D 게임 워크 플로우 학습
- 체스 로직 구현
- 로봇 팔 IK Animation 직접 구현

### 1. 체스 로직 구현

- 상속, 유틸 객체 등을 이용하여 기물 이동 로직을 재사용성 높게 작성
- 상태 패턴을 사용하여 플레이어와 AI의 턴 배분
- 체스 AI 연산은 ConcurrentQueue로 비동기 처리

### 2. 로봇 팔 IK Animation 직접 구현

- 로봇 팔이 체스 말을 집은 뒤 자연스럽게 이동시키는 애니메이션이 필요했음
- 유니티 내장 IK 솔루션은 Humanoid Avatar에만 적용되고, FABRIK과 같은 알고리즘은 예외 상황에서의 오작동이 일어날 수 있어 직접 구현하기로 함
- 제한 조건에 따른 각 관절의 위치를 수식으로 정리하여 목표 위치를 계산
- Command 패턴을 사용하여 애니메이션 순차적 실행
