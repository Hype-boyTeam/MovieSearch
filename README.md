# MovieSearch API
[![master](https://github.com/Hype-boyTeam/MovieSearch/actions/workflows/build.yml/badge.svg)](https://github.com/Hype-boyTeam/MovieSearch/actions/workflows/build.yml)

TODO: 설명, 검색기능 제공하는 서버

## 개발 환경 설정 및 실행
### 의존하고 있는 도구
- [elasticsearch](https://www.elastic.co/)
- [postgres](https://www.postgresql.org) (예정?)

위에 명시한 툴을 직접 설치하고 설정하는 과정을 생략할 수 있도록 간단한 [docker compose file](compose.yml)을 제공하고 있습니다.
1. [Docker Desktop](https://docs.docker.com/desktop/install/windows-install/)을 설치하세요.
1. 터미널에서 `docker compose --profile=dev up`을 입력하세요.
1. 종료할 때는 Ctrl+C를 누르면 됩니다.


### `appsettings.Development.json` 설정
#### ConnectionStrings
| Key | Notes |
|:----|:------|
| ElasticHost | elasticsearch가 실행되고 있는 호스트의 주소 |
| ElasticUser | 접속할 elasticsearch의 계정명 |
| ElasticPassword | 위 elastic 계정의 비밀번호 |
| Postgres | postgres connection string |


### MovieSearch 실행
필요한 개발 환경을 모두 설정하셨다면 터미널에서 `dotnet run --project MovieSearch`를 입력해 MovieSearch를 실행할 수 있습니다.
개발 모드로 실행되는 도중에는 http://localhost:5013/swagger 에 접속하여 제공하고 있는 Web API를 확인할 수 있습니다.

만약 소스 코드를 수정할 때 자동으로 변경 사항을 반영하려면 각 IDE에서 제공하는 hot-reload 기능을 이용하거나
터미널에서 `dotnet watch --project MovieSearch`를 입력하세요.

### 참고사항
master 브랜치에 커밋된 코드는 빌드에 성공하면 자동으로 https://api.peru0.com 에 반영됩니다.

## 문서
TODO.
