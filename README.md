# WhoCame-backend

[![license](https://img.shields.io/github/license/code-418-dpr/WhoCame-backend)](https://opensource.org/licenses/MIT)
[![release](https://img.shields.io/github/v/release/code-418-dpr/WhoCame-backend?include_prereleases)](https://github.com/code-418-dpr/WhoCame-backend/releases)
[![downloads](https://img.shields.io/github/downloads/code-418-dpr/WhoCame-backend/total)](https://github.com/code-418-dpr/WhoCame-backend/releases)
[![code size](https://img.shields.io/github/languages/code-size/code-418-dpr/WhoCame-backend.svg)](https://github.com/code-418-dpr/WhoCame-backend)

[![build](https://github.com/code-418-dpr/WhoCame-backend/actions/workflows/build.yaml/badge.svg)](https://github.com/code-418-dpr/WhoCame-backend/actions/workflows/build.yaml)

Бэкенд для проекта [WhoCame](https://github.com/code-418-dpr/WhoCame)

## Особенности реализации

- [x] Аутентификация и авторизация
- [ ] Стриминг видео по url адресу
- [ ] Стриминг видео по загруженному файлу
- [ ] Отправка потока данных на CV-сервис
- [ ] Принятие и обработка результатов CV-сервиса с занесением в бэк

## Стек

- **C#** — язык программирования
- ...
- **Docker** — платформа для контейнеризации

## Установка и запуск

0. Клонируйте репозиторий и перейдите в его папку.

### Посредством Docker

1. Установите Docker.
2. Создайте файл `.env` на основе [.env.template](.env.template) и настройте все описанные там параметры.
3. Запустите сборку образа:

```shell
docker build -t whocame_backend .
```

4. Теперь запускать образ можно командой:
```shell
docker run -d --name whocame_backend_standalone whocame_backend
```

### Без использования Docker

...

## Модификация

Если вы планируете модифицировать проект...
