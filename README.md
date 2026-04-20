# 🍔 Good Hamburger - Sistema de Pedidos

Sistema para registro de pedidos de uma lanchonete, construído com **C# / ASP.NET Core** no backend e deploy na **Azure**.

Conta com dois perfis de uso: um **painel administrativo para o restaurante** (estilo Kanban/KDS) e uma **interface de autoatendimento para o cliente** (estilo totem McDonald's) com notificações em tempo real.

---

## 📋 Sobre o Desafio

API REST para gerenciar pedidos de uma lanchonete com as seguintes regras de negócio:

### Cardápio

| Tipo           | Item         | Preço   |
| -------------- | ------------ | ------- |
| Sanduíche      | X Burger     | R$ 5,00 |
| Sanduíche      | X Egg        | R$ 4,50 |
| Sanduíche      | X Bacon      | R$ 7,00 |
| Acompanhamento | Batata Frita | R$ 2,00 |
| Acompanhamento | Refrigerante | R$ 2,50 |

### Regras de Desconto

| Combinação                        | Desconto |
| --------------------------------- | -------- |
| Sanduíche + Batata + Refrigerante | 20%      |
| Sanduíche + Refrigerante          | 15%      |
| Sanduíche + Batata                | 10%      |

### Restrições

- Cada pedido pode conter **apenas um sanduíche, uma batata e um refrigerante**
- Itens duplicados devem retornar mensagem de erro clara

---

## 👥 Tipos de Usuário e Fluxos

### 🏪 Usuário Restaurante (Admin)

Acesso protegido com **login e senha simples**. É quem gerencia os pedidos que chegam.

**Fluxo:**

```
Login → Painel Kanban (KDS) → Gerenciar pedidos por status → Logout
```

**Funcionalidades:**

- Autenticação simples (email + senha, JWT)
- **Painel Kanban / KDS (Kitchen Display System)** com colunas:

| Coluna        | Descrição                                   |
| ------------- | ------------------------------------------- |
| 📥 Recebido   | Pedido acabou de chegar (cliente finalizou) |
| 🔥 Em Preparo | Cozinha iniciou a preparação                |
| ✅ Pronto     | Pedido finalizado, aguardando retirada      |
| 📦 Entregue   | Cliente retirou (histórico)                 |

- **Drag & drop** para mover pedidos entre colunas
- Ao mover para "Pronto", **dispara notificação em tempo real** para o cliente
- Visualização dos detalhes do pedido (itens, valor, desconto aplicado)
- Indicador visual de tempo (quanto tempo o pedido está em cada etapa)

> 💡 **Referência real**: Restaurantes usam o chamado **KDS (Kitchen Display System)**, que funciona exatamente como um Kanban. O iFood, por exemplo, tem um painel parecido para os restaurantes parceiros. A ideia é a mesma.

---

### 🧑‍💻 Usuário Cliente (Autoatendimento)

Sem login. Interface de **totem de autoatendimento** estilo McDonald's.

**Fluxo completo:**

```
Tela Inicial → Escolher Sanduíche → Escolher Acompanhamentos → Resumo do Pedido
→ Tela de Pagamento (simulada) → Tela de Acompanhamento → Notificação de Pronto
```

**Telas do Cliente:**

#### 1. 🏠 Tela Inicial

- Boas-vindas com branding do restaurante
- Botão "Fazer Pedido"
- Mostrar painel de pedidos prontos (visível para todos)

#### 2. 🍔 Seleção do Sanduíche

- Cards visuais dos sanduíches (imagem, nome, preço)
- Seleção única (apenas 1 sanduíche)

#### 3. 🍟 Seleção de Acompanhamentos

- Cards de Batata Frita e Refrigerante
- Seleção opcional (pode pular)
- Preview do desconto que será aplicado conforme seleção

#### 4. 📋 Resumo do Pedido

- Lista de itens selecionados
- Subtotal, desconto aplicado (com indicação da regra), total final
- Botões "Voltar" e "Ir para Pagamento"

#### 5. 💳 Tela de Pagamento (Simulada)

- Simulação visual de pagamento (animação de processamento)
- Não há integração real com gateway de pagamento
- Após "pagamento", pedido é criado na API com status "Recebido"

#### 6. 📺 Tela de Acompanhamento (Estilo McDonald's)

- **Dois painéis lado a lado:**

| Painel Esquerdo          | Painel Direito             |
| ------------------------ | -------------------------- |
| 🔄 **Em Preparo**        | ✅ **Pronto para Retirar** |
| Lista de números/códigos | Lista de números/códigos   |

- O número do pedido do cliente fica **destacado**
- **Atualização em tempo real** via SignalR (sem refresh)
- Quando o pedido muda para "Pronto", exibe **notificação visual + sonora**

---

## 🔔 Comunicação em Tempo Real (SignalR)

O SignalR será usado para manter o frontend sincronizado com o backend sem polling.

### Eventos do Hub

| Evento               | Direção         | Descrição                                          |
| -------------------- | --------------- | -------------------------------------------------- |
| `OrderStatusChanged` | Server → Client | Pedido mudou de status (ex: Em Preparo → Pronto)   |
| `NewOrderReceived`   | Server → Client | Novo pedido chegou (notifica o painel do admin)    |
| `OrderReady`         | Server → Client | Pedido específico está pronto (notifica o cliente) |

### Grupos SignalR

- **`admin`** — todos os usuários do restaurante recebem atualizações do Kanban
- **`order-{id}`** — cliente específico acompanhando seu pedido
- **`display`** — tela geral de acompanhamento (painel McDonald's)

---

## 🏗️ Arquitetura - 4 Camadas

```
📦 good-hamburger/                        (monorepo)
├── 📂 .github/
│   └── 📂 workflows/
│       ├── 📄 backend-ci.yml             → CI/CD backend (path filter: backend/**)
│       └── 📄 frontend-ci.yml            → CI/CD frontend (path filter: frontend/**)
├── 📂 backend/
│   ├── 📂 GoodHamburger.API/             → Camada de Apresentação (Controllers, Hubs, Middlewares)
│   ├── 📂 GoodHamburger.Application/     → Camada de Aplicação (Services, DTOs, Validators)
│   ├── 📂 GoodHamburger.Domain/          → Camada de Domínio (Entities, Enums, Regras de Negócio)
│   ├── 📂 GoodHamburger.Infrastructure/  → Camada de Infraestrutura (EF Core, Repositories)
│   ├── 📂 GoodHamburger.Tests/           → Testes Unitários e de Integração
│   └── 📄 GoodHamburger.sln
├── 📂 frontend/
│   └── (projeto Blazor WASM)
└── 📄 README.md
```

### Responsabilidades por Camada

#### 1. `GoodHamburger.Domain` (sem dependências)

- **Entities**: `Order`, `OrderItem`, `MenuItem`, `User`
- **Enums**: `MenuItemType` (Sandwich, Side, Drink), `OrderStatus` (Received, Preparing, Ready, Delivered), `UserRole` (Admin)
- **Value Objects / Regras**: lógica de cálculo de desconto
- **Interfaces**: `IOrderRepository`, `IMenuItemRepository`, `IUserRepository`

#### 2. `GoodHamburger.Application` (depende de Domain)

- **DTOs**: `CreateOrderRequest`, `UpdateOrderRequest`, `OrderResponse`, `LoginRequest`, `LoginResponse`
- **Services**: `OrderService`, `MenuService`, `AuthService`
- **Interfaces**: `IOrderService`, `IMenuService`, `IAuthService`, `INotificationService`
- **Validators**: validação de itens duplicados, pedido válido
- **Mapping**: AutoMapper profiles (Entity ↔ DTO)

#### 3. `GoodHamburger.Infrastructure` (depende de Domain)

- **DbContext**: `GoodHamburgerDbContext` (Entity Framework Core)
- **Repositories**: `OrderRepository`, `MenuItemRepository`, `UserRepository`
- **Services**: `NotificationService` (implementação SignalR de `INotificationService`)
- **Migrations**: versionamento do banco
- **Seed Data**: popular cardápio inicial + usuário admin padrão
- **Configurations**: Fluent API para mapeamento das entidades

#### 4. `GoodHamburger.API` (depende de Application e Infrastructure)

- **Controllers**: `OrdersController`, `MenuController`, `AuthController`
- **Hubs**: `OrderHub` (SignalR para tempo real)
- **Middlewares**: tratamento global de exceções, autenticação JWT
- **Program.cs**: DI container, Swagger, CORS, SignalR config
- **appsettings.json**: connection strings, JWT secrets

#### 5. `GoodHamburger.Tests`

- Testes unitários das regras de desconto
- Testes unitários dos services
- Testes de integração dos endpoints (opcional: WebApplicationFactory)

---

## 🔌 Endpoints da API

### Autenticação

| Método | Rota              | Auth | Descrição            |
| ------ | ----------------- | ---- | -------------------- |
| `POST` | `/api/auth/login` | ❌   | Login do restaurante |

### Cardápio

| Método | Rota        | Auth | Descrição       |
| ------ | ----------- | ---- | --------------- |
| `GET`  | `/api/menu` | ❌   | Listar cardápio |

### Pedidos — Cliente (Totem)

| Método | Rota                       | Auth | Descrição                                    |
| ------ | -------------------------- | ---- | -------------------------------------------- |
| `POST` | `/api/orders`              | ❌   | Criar novo pedido                            |
| `GET`  | `/api/orders/{id}`         | ❌   | Consultar pedido por ID                      |
| `GET`  | `/api/orders/track/{code}` | ❌   | Acompanhar pedido pelo código                |
| `GET`  | `/api/orders/display`      | ❌   | Pedidos para o painel (em preparo + prontos) |

### Pedidos — Restaurante (Admin)

| Método   | Rota                            | Auth | Descrição                                  |
| -------- | ------------------------------- | ---- | ------------------------------------------ |
| `GET`    | `/api/admin/orders`             | 🔒   | Listar todos os pedidos (Kanban)           |
| `PATCH`  | `/api/admin/orders/{id}/status` | 🔒   | Alterar status do pedido (mover no Kanban) |
| `GET`    | `/api/admin/orders/{id}`        | 🔒   | Detalhes completos do pedido               |
| `PUT`    | `/api/admin/orders/{id}`        | 🔒   | Atualizar pedido                           |
| `DELETE` | `/api/admin/orders/{id}`        | 🔒   | Remover pedido                             |

### SignalR Hub

| Rota           | Descrição                           |
| -------------- | ----------------------------------- |
| `/hubs/orders` | Hub para atualizações em tempo real |

---

## 🛠️ Stack Tecnológica

### Backend

- **C# / .NET 8** (ou versão mais recente LTS)
- **ASP.NET Core Web API**
- **Entity Framework Core** (Code First)
- **SignalR** (comunicação em tempo real)
- **JWT Bearer** (autenticação)
- **AutoMapper** (mapeamento Entity ↔ DTO)
- **FluentValidation** (validações)
- **Swagger / OpenAPI** (documentação)
- **xUnit + Moq** (testes)

### Banco de Dados

- **SQL Server** (Azure SQL Database em produção)
- **InMemory Provider** (para testes)

### Frontend (mesma repo, pasta `/frontend`)

- **Blazor WebAssembly** (SPA no navegador)
- **SignalR Client** (tempo real)
- Duas interfaces no mesmo app:
  - `/admin` → Painel Kanban (protegido com login)
  - `/` → Totem de autoatendimento (público)

---

## ☁️ Deploy - Azure

### Monorepo com Deploys Independentes

Um único repositório, mas cada parte tem sua **pipeline separada** com path filters:

```
📦 good-hamburger/
├── backend/**   → dispara backend-ci.yml  → deploy Azure App Service
├── frontend/**  → dispara frontend-ci.yml → deploy Azure Static Web Apps
```

### Recursos Azure

| Recurso                   | Uso                                               |
| ------------------------- | ------------------------------------------------- |
| **Azure App Service**     | Hospedar a API + SignalR                          |
| **Azure SQL Database**    | Banco de dados relacional                         |
| **Azure Static Web Apps** | Hospedar o frontend Blazor WASM                   |
| **Azure SignalR Service** | Gerenciar conexões WebSocket em escala (opcional) |
| **GitHub Actions**        | CI/CD pipelines (2 workflows independentes)       |

### Pipeline CI/CD — Backend (`backend-ci.yml`)

Dispara apenas quando há mudanças em `backend/**`:

```yaml
name: Backend CI/CD
on:
  push:
    branches: [main]
    paths: ['backend/**']
  pull_request:
    paths: ['backend/**']

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
        working-directory: ./backend
      - run: dotnet build --no-restore
        working-directory: ./backend
      - run: dotnet test --no-build --verbosity normal
        working-directory: ./backend

  deploy:
    needs: build-and-test
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet publish -c Release -o ./publish
        working-directory: ./backend/GoodHamburger.API
      - uses: azure/webapps-deploy@v2
        with:
          app-name: 'good-hamburger-api'
          package: ./backend/GoodHamburger.API/publish
```

### Pipeline CI/CD — Frontend (`frontend-ci.yml`)

Dispara apenas quando há mudanças em `frontend/**`:

```yaml
name: Frontend CI/CD
on:
  push:
    branches: [main]
    paths: ['frontend/**']
  pull_request:
    paths: ['frontend/**']

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet publish -c Release -o ./publish
        working-directory: ./frontend
      - uses: Azure/static-web-apps-deploy@v1
        with:
          app_location: './frontend/publish/wwwroot'
```

### Resultado

```
Commit no backend/  → roda APENAS backend-ci.yml  → testes + deploy API
Commit no frontend/ → roda APENAS frontend-ci.yml → build + deploy SWA
Commit nos dois     → roda AMBOS em paralelo
```

---

## 📐 Decisões de Arquitetura

### Estrutura e Domínio
1. **4 Camadas** — separação clara de responsabilidades, facilita testes e manutenção
2. **Domain sem dependências** — regras de negócio isoladas, testáveis sem infra
3. **Repository Pattern** — abstração do acesso a dados via interfaces no Domain
4. **Desconto no Domain** — lógica de cálculo de desconto vive na camada de domínio
5. **Seed Data** — cardápio fixo + admin padrão populados via migration/seed
6. **Validação em duas camadas** — FluentValidation na Application + Data Annotations nos DTOs
7. **Global Exception Handler** — middleware para padronizar respostas de erro

### Tempo Real e Notificações
8. **SignalR com 2 grupos** — `display` (clientes) e `admin` (restaurante), sem grupo individual por pedido
9. **Filtragem no frontend** — SignalR faz broadcast geral, cada cliente filtra localmente pelo seu código
10. **DB primeiro, SignalR depois** — banco é a fonte da verdade; notificação é best-effort com try/catch
11. **Polling como fallback** — frontend faz GET a cada ~30s caso SignalR falhe, garantindo eventual consistency

### Autenticação e UX
12. **JWT simples** — autenticação stateless apenas para o perfil restaurante
13. **Cliente sem sessão** — código do pedido persiste apenas no frontend (localStorage + URL)
14. **Pagamento simulado** — foco na experiência do fluxo, sem integração real com gateway
15. **Código do pedido** — geração de código curto amigável (ex: `#A42`) para exibição no totem/painel
16. **Duas interfaces, um frontend** — Blazor WASM com rotas separadas (`/` para totem, `/admin` para Kanban)

### Repositório e CI/CD
17. **Monorepo** — backend e frontend no mesmo repo; mais simples pra um dev solo, um link só pro portfolio
18. **Path filters no GitHub Actions** — workflows independentes por pasta, deploy separado mesmo no monorepo
19. **CI obrigatório em PR** — testes rodam automaticamente em pull requests antes do merge

---

## ✅ Checklist de Implementação

### Fase 1 — Setup do Monorepo e Domínio

- [ ] Criar estrutura do monorepo (`backend/`, `frontend/`, `.github/workflows/`)
- [ ] Criar Solution com os 5 projetos em `backend/`
- [ ] Definir entidades (`Order`, `OrderItem`, `MenuItem`, `User`)
- [ ] Implementar enums (`MenuItemType`, `OrderStatus`, `UserRole`)
- [ ] Implementar lógica de cálculo de desconto no Domain
- [ ] Implementar geração de código do pedido
- [ ] Definir interfaces dos repositórios

### Fase 2 — Infraestrutura

- [ ] Configurar `DbContext` com Fluent API
- [ ] Implementar repositórios
- [ ] Criar Seed Data (cardápio + admin padrão)
- [ ] Gerar migration inicial
- [ ] Implementar `NotificationService` (SignalR)

### Fase 3 — Application

- [ ] Criar DTOs de request/response
- [ ] Implementar `OrderService` e `MenuService`
- [ ] Implementar `AuthService` (login simples + JWT)
- [ ] Configurar AutoMapper profiles
- [ ] Implementar validações com FluentValidation

### Fase 4 — API

- [ ] Criar Controllers (Orders, Menu, Auth, Admin)
- [ ] Implementar `OrderHub` (SignalR)
- [ ] Configurar DI, Swagger, CORS, JWT no `Program.cs`
- [ ] Implementar middleware de exceções
- [ ] Proteger rotas admin com `[Authorize]`

### Fase 5 — Testes

- [ ] Testes unitários das regras de desconto
- [ ] Testes unitários dos services
- [ ] Testes de integração dos endpoints
- [ ] Testes de validação (itens duplicados, pedido inválido)

### Fase 6 — CI/CD (GitHub Actions)

- [ ] Criar `backend-ci.yml` (build + test + deploy)
- [ ] Criar `frontend-ci.yml` (build + deploy)
- [ ] Testar path filters (commit só no backend, só no frontend, nos dois)
- [ ] Configurar secrets do Azure no GitHub

### Fase 7 — Deploy Azure

- [ ] Configurar Azure SQL Database
- [ ] Deploy da API no App Service
- [ ] Configurar Azure SignalR Service (opcional)
- [ ] Deploy do frontend no Static Web Apps

### Fase 8 — Frontend Blazor (pasta `/frontend`)

- [ ] Setup do projeto Blazor WASM em `frontend/`
- [ ] **Totem — Cliente:**
  - [ ] Tela inicial (boas-vindas + painel de prontos)
  - [ ] Tela de seleção de sanduíche
  - [ ] Tela de seleção de acompanhamentos (com preview de desconto)
  - [ ] Tela de resumo do pedido
  - [ ] Tela de pagamento simulado
  - [ ] Tela de acompanhamento em tempo real
  - [ ] Notificação visual + sonora quando pedido pronto
- [ ] **Admin — Restaurante:**
  - [ ] Tela de login
  - [ ] Painel Kanban (drag & drop entre colunas)
  - [ ] Detalhes do pedido no card
  - [ ] Indicador de tempo por etapa
  - [ ] Notificação de novo pedido

---

## 🚀 Como Rodar Localmente

```bash
# Clonar o repositório
git clone https://github.com/seu-usuario/good-hamburger.git
cd good-hamburger
```

### Backend

```bash
# Restaurar dependências
dotnet restore ./backend

# Aplicar migrations
dotnet ef database update \
  --project ./backend/GoodHamburger.Infrastructure \
  --startup-project ./backend/GoodHamburger.API

# Rodar a API
dotnet run --project ./backend/GoodHamburger.API

# Rodar testes
dotnet test ./backend
```

> A API estará disponível em `https://localhost:5001` com Swagger em `/swagger`

### Frontend

```bash
# Rodar o Blazor WASM
dotnet run --project ./frontend
```

> O frontend estará disponível em `https://localhost:5002`

---

## 📝 Observações

- O cardápio é **fixo** e vem pré-populado (seed). Não há CRUD de menu items.
- O endpoint `GET /api/menu` apenas consulta os itens disponíveis.
- O **pagamento é simulado** — não há integração com gateway de pagamento.
- O **cliente não precisa de cadastro/login** — a experiência é igual a um totem de lanchonete.
- O **admin** tem login simples (usuário padrão criado via seed).
- Foco principal é demonstrar **organização de código**, **modelagem do domínio**, **decisões técnicas** e **experiência do usuário**.
- Prazo sugerido do desafio: **7 dias corridos**.
