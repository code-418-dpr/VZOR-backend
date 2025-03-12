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
- [x] Проверка консистентности загруженных файлов в S3 и Postgres через Hangfire 
- [ ] Сервис уведомлений
- [ ] Связь с сервисом уведомлений через RabbitMQ
- [ ] Файловый сервис
- [ ] Подтверждение учётной записи через почту
- [ ] Загрузка фотографий в S3, PostgreSQL и отправка на CV через gRPC
- [ ] Принятие и обработка результатов CV-сервиса с занесением мета-данных в БД
- [ ] Авторизация через oauth 2(Yandex,VK,Google)

## Стек

- **C#** — язык программирования
- **Postgres** — реляционная база данных
- **MongoDb** — NoSql база данных для хранения метаданных о изображениях
- **Seq** — сервис логгирования
- **gRPC** — протокол взаимодействия между бэкендом и сервисом CV
- **RabbitMQ** — брокер сообщений для взаимодействия между бэкендом и сервисом уведомлений
- **Redis** — NoSql база данных для кэширование объектов
- **Minio** — облачное S3 хранилище объектов
- **Docker** — платформа для контейнеризации

## Библиотеки

| Библиотека | Версия | Источник |
| --- | --- | --- |
| Grpc.AspNetCore | 2.64.0, 2.67.0 | nuget.org |
| Grpc.Tools | 2.70.0 (2.64.0) | nuget.org |
| Hangfire.AspNetCore | 1.8.18 | nuget.org |
| Hangfire.Core | 1.8.18 | nuget.org |
| Hangfire.PostgreSql | 1.20.10 | nuget.org |
| Microsoft.AspNetCore.Authentication.JwtBearer | 9.0.2 | nuget.org |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 9.0.2 | nuget.org |
| Microsoft.AspNetCore.Mvc.Core | 2.3.0 | nuget.org |
| Microsoft.AspNetCore.OpenApi | 9.0.2 | nuget.org |
| Microsoft.EntityFrameworkCore | 9.0.2 | nuget.org |
| Microsoft.EntityFrameworkCore.Design | 9.0.2 | nuget.org |
| Microsoft.EntityFrameworkCore.Tools | 9.0.2 | nuget.org |
| Microsoft.Extensions.Configuration | 9.0.2 | nuget.org |
| Minio | 6.0.4 | nuget.org |
| Newtonsoft.Json | 13.0.3 (11.0.1) | VS Offline nuget.org |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.4 | nuget.org |
| OpenTelemetry.Instrumentation.Process | 1.11.0-beta.1 | - |
| Scrutor | 6.0.1 | nuget.org |
| Serilog.AspNetCore | 9.0.0 | nuget.org |
| Serilog.Enrichers.Environment | 3.0.1 | nuget.org |
| Serilog.Enrichers.Thread | 4.0.0 | nuget.org |
| Serilog.Exceptions | 8.4.0 | nuget.org |
| Serilog.Sinks.Seq | 9.0.0 | nuget.org |
| Swashbuckle.AspNetCore | 7.3.1 | nuget.org |

## Установка и запуск

0. Клонируйте репозиторий и перейдите в его папку.

### Посредством Docker

1. Установите Docker.
2. Установите .NET SDK, а также EF Core. Последний можно добавить командой:

```shell
dotnet tool install --global dotnet-ef
```

3. Настройте файл [appsetting.Docker.json](src/VZOR.Web/appsettings.Docker.json), прописав собственные строки
   подключения (они должны совпадать с указанными в [compose.yaml](compose.yaml))
4. Создайте файл `.env`  и настройте все описанные там параметры.

5. Запустите сборку и подъём контейнера:

```shell
docker-compose up -d --build
```
   
6. Создайте миграции к базе данных:

Воспользуйтесь заготовленным скриптом:

```shell
.\migrations-add-and-update-without-drop-and-remove.cmd
```

или

```shell
cd src
dotnet ef migrations add <название миграции> --startup-project .\VZOR.Web\ --project .\Accounts\VZOR.Accounts.Infrastructure\ --context AccountsDbContext
dotnet ef migrations add <название миграции> --startup-project .\VZOR.Web\ --project .\Images\VZOR.Images.Infrastructure\ --context ApplicationDbContext
cd ..
```

В будущем, если миграции не применятся автоматически (при создании базы данных), их можно применить вручную:

```shell
cd src
dotnet ef database update --startup-project .\VZOR.Web\ --project .\Accounts\VZOR.Accounts.Infrastructure\ --context AccountsDbContext
dotnet ef database update --startup-project .\VZOR.Web\ --project .\Images\VZOR.Images.Infrastructure\ --context ApplicationDbContext
cd ..
```


Теперь можно использовать бэкенд по адресу http://localhost:8080. Документация к бэкенду доступна в
интерфейсе [Swagger](http://localhost:8080/swagger).
Также доступен dashboard Hangfire`a по адресу [Hangfire](http://localhost:8080/hangfire).

