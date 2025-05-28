# КПО ДЗ-2
## Хромова Елизавета БПИ2310

## Использовано

- **Чистая архитектура (Clean Architecture):**
- **SOLID:**
- **Dependency Injection:**
- **Repository Pattern:**
- **Separation of Concerns:**


## Краткое описание сервисов

### 1. **API Gateway**
- Единая точка входа для всех запросов, маршрутизация

### 2. **File Storage Service**
- Загрузка, хранение, выдача файлов, метаданные в PostgreSQL, файлы в volume, реализация паттерна Repository

### 3. **File Analysis Service**
- Анализ текста, бизнес-логика анализа

### Доступ к сервисам
- File Storage Service: http://localhost:7001/swagger
- File Analysis Service: http://localhost:7002/swagger
- API Gateway: http://localhost:7003

## Инструкция по запуску

### Предварительные требования
- Docker
- Docker Compose
- .NET 8.0 SDK

### Запуск системы

```bash
dotnet build
docker-compose up --build
```

### Остановка системы
```bash
docker-compose down
```

### Очистка данных
```bash
docker-compose down -v
```
