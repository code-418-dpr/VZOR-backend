# VZOR-backend

[![license](https://img.shields.io/github/license/code-418-dpr/VZOR-backend)](https://opensource.org/licenses/MIT)
[![release](https://img.shields.io/github/v/release/code-418-dpr/VZOR-backend?include_prereleases)](https://github.com/code-418-dpr/VZOR-backend/releases)
[![downloads](https://img.shields.io/github/downloads/code-418-dpr/VZOR-backend/total)](https://github.com/code-418-dpr/VZOR-backend/releases)
[![code size](https://img.shields.io/github/languages/code-size/code-418-dpr/VZOR-backend.svg)](https://github.com/code-418-dpr/VZOR-backend)

[![build](https://github.com/code-418-dpr/VZOR-backend/actions/workflows/build.yaml/badge.svg)](https://github.com/code-418-dpr/VZOR-backend/actions/workflows/build.yaml)
[![CodeQL (C#, GH Actions)](https://github.com/code-418-dpr/VZOR-backend/actions/workflows/codeql.yaml/badge.svg)](https://github.com/code-418-dpr/VZOR-backend/actions/workflows/codeql.yaml)

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
2. Настройте файл [appsetting.Docker.json](src/VZOR.Web/appsettings.Docker.json), прописав собственные строки
   подключения (они должны совпадать с указанными в [compose.yaml](compose.yaml))
3. Создайте файл `.env` на основе [.env.template](.env.template) и настройте все описанные там параметры.
4. Запустите контейнер:

```shell
docker-compose up -d
```

Теперь можно использовать бэкенд по адресу http://localhost:8080. Документация к бэкенду доступна в
интерфейсе [Swagger](http://localhost:8080/swagger).

