# TaskManager Backend

API REST do TaskManager, desenvolvida em .NET 10 com arquitetura em camadas, autenticação JWT e PostgreSQL.

## Stack

- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core
- PostgreSQL 17
- Npgsql
- JWT Bearer
- BCrypt
- FluentValidation
- Mapster
- Swagger
- Docker
- NUnit

## Funcionalidades

- Cadastro e autenticação de usuários
- Autenticação baseada em JWT
- Gerenciamento de tarefas
- Controle de acesso por usuário
- Paginação
- Filtros
- Ordenação
- Relatórios de tarefas
- Validação de dados
- Rate limiting
- Tratamento global de exceções
- Health check
- CORS
- Persistência em PostgreSQL

## Arquitetura

O projeto utiliza separação por responsabilidades:

```text
TaskManager
├── TaskManager/
│   └── API
├── TaskManager.Application/
│   └── Regras de aplicação, Services, DTOs e validações
├── TaskManager.Core/
│   └── Entidades, contratos e regras centrais
├── TaskManager.Infra/
│   └── EF Core, PostgreSQL, Repositories e infraestrutura
└── TaskManager.Tests/
    └── Testes automatizados
```

Fluxo principal de uma requisição:

```text
HTTP Request
    ↓
Controller
    ↓
Service
    ↓
Repository
    ↓
Entity Framework Core
    ↓
PostgreSQL
```

## Requisitos

- .NET 10 SDK
- Docker Desktop ou Rancher Desktop
- PostgreSQL 17, caso não utilize Docker

## Configuração

O ambiente local utiliza variáveis definidas no arquivo `.env`.

Exemplo:

```env
DB_NAME=task_manager
DB_USER=postgres
DB_PASSWORD=your_password
JWT_KEY=your_secret_key
```

Não versionar senhas, chaves JWT ou outros secrets reais.

## Execução com Docker Compose

Suba os serviços:

```bash
docker compose up --build
```

API:

```text
http://localhost:7102
```

PostgreSQL:

```text
localhost:5432
```

Para encerrar:

```bash
docker compose down
```

Os dados do PostgreSQL são armazenados no volume Docker `taskmanager_postgres_data`.

## Execução sem Docker

```bash
dotnet restore
dotnet run --project TaskManager/TaskManager.Api.csproj
```

## Banco de dados

O projeto utiliza PostgreSQL através do Entity Framework Core e Npgsql.

As migrations versionam a estrutura do banco.

Criar uma migration:

```bash
dotnet ef migrations add NomeDaMigration --project TaskManager.Infra --startup-project TaskManager
```

Aplicar migrations:

```bash
dotnet ef database update --project TaskManager.Infra --startup-project TaskManager
```

## Swagger

O Swagger é habilitado em ambientes que não são de produção.

```text
http://localhost:7102/swagger
```

## Health Check

```http
GET /health
```

Resposta esperada:

```text
Healthy
```

## Autenticação

A autenticação utiliza JWT Bearer.

As requisições protegidas devem enviar:

```http
Authorization: Bearer {token}
```

A identificação do usuário autenticado é utilizada para garantir que as operações de tarefas sejam realizadas dentro do contexto do usuário.

## Validação e tratamento de erros

As entradas da API são validadas com FluentValidation.

Exceções não tratadas pelos endpoints são processadas pelo handler global da API e convertidas para respostas HTTP padronizadas.

A API também possui rate limiting.

## Testes

```bash
dotnet test
```

Os testes ficam no projeto:

```text
TaskManager.Tests
```

## Estrutura de uma nova funcionalidade

1. Criar ou alterar as entidades necessárias no Core.
2. Criar DTOs e contratos na Application.
3. Implementar as regras de negócio em Services.
4. Implementar o acesso a dados em Repositories quando necessário.
5. Configurar os mapeamentos com Mapster.
6. Criar o Controller e os endpoints.
7. Adicionar validações.
8. Criar ou atualizar migrations quando houver alteração no banco.
9. Criar os testes.
10. Validar a integração com o frontend.

## Docker

O Dockerfile da API está em:

```text
TaskManager/Dockerfile
```

O container utiliza a porta interna `8080`.

O Docker Compose expõe a API localmente na porta `7102`.

## Deploy

A aplicação pode ser executada em ambientes compatíveis com containers Docker.

Em produção, as configurações sensíveis devem ser fornecidas através das variáveis de ambiente do serviço de hospedagem.

## Licença

Projeto desenvolvido para fins de estudo e portfólio.
