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
        - 학번 비밀번호 받아서 로그인 진행
    - /auth/logout
        - GET Action, Authorized
        - 세션 쿠키 삭제
    - /auth/register
        - POST Action
        - 학번 비밀번호 학번 받아서 회원가입 진행
    - /auth/remove
        - POST Action
        - 학번 비밀번호 받아서 DB에서 해당 회원 제거
    - /auth/recover
        - POST Action, Administer Role required
        - 학번 받아서 비밀번호 password로 초기화
- /project
- /admin

---

## DB Scheme

### User

| Id | Name | StudentNumber | PasswordHash | Salt | IsVerified | Authority |
| --- | --- | --- | --- | --- | --- | --- |
| 고유키 |  |  |  |  |  |  |

### Project

| ProjectId | Name | Description | StartDate | EndDate |
| --- | --- | --- | --- | --- |
| 고유키 |  |  |  |  |

### Milestone

| Id | Name | Description | IsFinished | DueDate | ProjectId |
| --- | --- | --- | --- | --- | --- |
| 고유키 |  |  |  |  | 외래키 |

### Image

| ImageSrc | ProjectId |
| --- | --- |
|  | 외래키 |

### Member

| StudentNumber | Role | ProjectId |
| --- | --- | --- |
|  |  | 외래키 |

### StudentNum

| StudentNumber | Name |
| --- | --- |
| 외래키 |  |