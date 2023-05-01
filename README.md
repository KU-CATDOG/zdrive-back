# zdrive-back
캣독 프로젝트 관리 웹 프로젝트 백엔드

SessionStorage 구현해야 할 것??
    세션 추가
    SSID로 세션 확인
    세션 만료되면 없어지게
    세션 수동 제거

세션엔?
    유저 DB 기본키
    SSID
    만료 시간

Unit Test?
    새로운 유저가 들어오면 세션 추가해야함(true 리턴하고 SSID out)
    이미 세션에 있는 유저가 들어오면 세션 추가 메소드가 실패해야함(false 리턴)
    기존 유저를 확인 전 만료된 세션을 제거해야함

    받은 SSID가 세션에 있을 경우 유저 DB 기본키를 리턴해야함
    받은 SSID가 세션에 없을 경우 null을 리턴해야함
    받은 SSID가 만료된 경우 null을 리턴해야함

    SSID가 있을 경우 그 세션을 제거함
    SSID가 없을 경우 Exception 발생