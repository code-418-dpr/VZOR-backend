# VZOR-backend

[![license](https://img.shields.io/github/license/code-418-dpr/WhoCame-backend)](https://opensource.org/licenses/MIT)
[![release](https://img.shields.io/github/v/release/code-418-dpr/WhoCame-backend?include_prereleases)](https://github.com/code-418-dpr/WhoCame-backend/releases)
[![downloads](https://img.shields.io/github/downloads/code-418-dpr/WhoCame-backend/total)](https://github.com/code-418-dpr/WhoCame-backend/releases)
[![code size](https://img.shields.io/github/languages/code-size/code-418-dpr/WhoCame-backend.svg)](https://github.com/code-418-dpr/WhoCame-backend)

[![build](https://github.com/code-418-dpr/WhoCame-backend/actions/workflows/build.yaml/badge.svg)](https://github.com/code-418-dpr/WhoCame-backend/actions/workflows/build.yaml)
[![CodeQL (C#, GH Actions)](https://github.com/code-418-dpr/WhoCame-backend/actions/workflows/codeql.yaml/badge.svg)](https://github.com/code-418-dpr/WhoCame-backend/actions/workflows/codeql.yaml)

Бэкенд для проекта [VZOR](https://github.com/code-418-dpr/VZOR)

## Особенности реализации

- [x] Аутентификация и авторизация
- [x] Настройка профиля пользователя
- [ ] Сервис уведомлений
- [ ] Связь с сервисом уведомлений через RabbitMQ
- [ ] Файловый сервис
- [ ] Подтверждение учётной записи через почту
- [ ] Загрузка фотографий в S3, PostgreSQL и отправка на CV через gRPC
- [ ] Принятие и обработка результатов CV-сервиса с занесением мета-данных в БД

## Стек

- **C#** — язык программирования
- ...
- **Docker** — платформа для контейнеризации

## Установка и запуск

0. Клонируйте репозиторий и перейдите в его папку.

### Посредством Docker

1. Установите Docker.
2. Настройте appsetting.Docker.json файл, прописав собственные строки подключения(они должны совпадать с указанными в docker-compose)

Пример:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=host;Port=port;Database=database;User Id=user;Password=password;",
    "Seq": "http://seq:port",
    "Redis": "redis:port"
  },
  "Minio": {
    "Endpoint": "minio:port",
    "AccessKey": "minioadmin", 
    "SecretKey": "minioadmin",
    "WithSsl": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware": "Information"
    }
  },
  "RefreshSession": {
    "ExpiredDaysTime": 30
  },
  "Jwt": {
    "Issuer": "http://minoddein-company/api",
    "Audience": "https://minoddein-company.ru",
    "Key": "samndoiasnd089i32ni9w09jds9c9020masopdcmao",
    "ExpiredMinutesTime": 60
  },
  "EntityDeletion": {
    "ExpiredDays": 30
  },
  "AllowedHosts": "*"
}

```

3. Создайте файл `.env`  и настройте все описанные там параметры.

```shell
ADMIN__USERNAME=admin
ADMIN__EMAIL=adming@admin.com
ADMIN__PASSWORD=adming
```

4. Запустите сборку и подъём образа:

```shell
docker-compose up -d
```

5. Теперь вы можете использовать backend, работающий через адрес http://localhost:8080, а также через swagger  http://localhost:8080/swagger

