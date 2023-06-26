# ZDrive-Back

캣독 프로젝트 관리 및 웹 프로젝트 백앤드

---

## Purposes

프로젝트의 보관 (포폴용, 포스트모템)
과거플젝의 구경 -> 홍보에도 쓸수있고
일정관리툴도 포함? 마일스톤 단위로

---

## Features

로그인창
메인화면
프로젝트 관리, 열람 (인원, 게임개요, 스크린샷)
(프로젝트 진행도 관리 (목표날짜 퍼센트 목표 등))
관리창 (회원가입 승인, 글삭제 등)

사용자
ID 비밀번호 이름 전화번호 학번 이메일 권한정도(일반사용자, 관리자) passwordsalt

프로젝트
이름 진행시기 인원수(인원 리스트) 소개문 스크린샷 현재퍼센트 진행상태

진행도
프로젝트ID 진행퍼센트 진행목표글 목표날짜

---

### API List

- /auth
  - /auth/login
    - POST Action
    - {StudentNumber: 학번, Password: 패스워드}
    - 로그인 실패 시 NotFound
    - 인증안된 회원일 때 Forbid
  - /auth/logout
    - GET Action, 회원 권한 필요
    - 세션 쿠키 삭제
    - SSID 쿠키 없으면 NotFound
    - SSID 쿠키 형식 오류 시 BadRequest
    - 없는 SSID면 NotFound
  - /auth/register
    - POST Action
    - {Name: 이름, StudentNumber: 학번, Password: 패스워드}
    - 학번 이미 있으면 Conflict
  - /auth/remove
    - POST Action
    - 학번 비밀번호 받아서 DB에서 해당 회원 제거
    - {StudentNumber: 학번, Password: 패스워드}
    - 학번 없거나 비밀번호 틀리면 NotFound
  - /auth/recover
    - POST Action, 어드민 권한 필요
    - 학번 받아서 비밀번호 password로 초기화
    - 미구현
- /project
  - /project/{id}
    - GET Action
    - 해당 id 프로젝트 정보 가져옴
    - id 없으면 NotFound
  - /project/list
    - GET Action
    - 모든 프로젝트 List 가져옴
- /admin

---

# Production Setting에 대해

## ZDrive/appsettings.Production.json

이중 ConnectionStrings에서 DataSource의 위치를 변경해주어야 합니다

> DataSource=app.db => DataSource=/abc/def/app.db

해당 위치에 app.db의 sqlite db 파일이 필요합니다

```bash
$ dotnet run --environment Production
```

위의 코드를 통해 Production으로 실행됩니다
