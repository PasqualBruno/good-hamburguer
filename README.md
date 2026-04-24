# 🍔 Good Hamburger

Sistema de pedidos para lanchonete com promoções automáticas. Backend em **C# / .NET 10** com **SignalR** para tempo real, frontend em **React**.

---

## 📋 Cardápio

| Tipo           | Item         | Preço   |
| -------------- | ------------ | ------- |
| Sanduíche      | X Burger     | R$ 5,00 |
| Sanduíche      | X Egg        | R$ 4,50 |
| Sanduíche      | X Bacon      | R$ 7,00 |
| Acompanhamento | Batata Frita | R$ 2,00 |
| Bebida         | Refrigerante | R$ 2,50 |

---

## 🎉 Promoções

As promoções são aplicadas **automaticamente** com base nos itens do pedido.

| Promoção              | Condição                          | Desconto |
| --------------------- | --------------------------------- | -------- |
| **Combo Completo**    | Sanduíche + Batata + Refrigerante | 20%      |
| **Combo Verão**       | Sanduíche + Refrigerante          | 15%      |
| **Combo Barriga Cheia** | Sanduíche + Batata              | 10%      |

> A melhor promoção aplicável é escolhida automaticamente (maior desconto).

---

## ⚠️ Regras

- Cada pedido pode ter **no máximo 1 item de cada tipo** (1 sanduíche, 1 acompanhamento, 1 bebida)
- Itens são **opcionais** — o pedido precisa ter pelo menos 1 item
- Itens duplicados por tipo retornam erro com código específico

---

## 🔌 API — Endpoints

### Cardápio

| Método | Rota             | Descrição                                          |
| ------ | ---------------- | -------------------------------------------------- |
| `GET`  | `/api/menu`      | Lista todos os itens do cardápio com preços e tipos |

### Promoções

| Método | Rota               | Descrição                                                        |
| ------ | ------------------ | ---------------------------------------------------------------- |
| `GET`  | `/api/promotions`  | Lista todas as promoções com nomes, descontos, condições e itens |

### Pedidos

| Método | Rota          | Descrição                                                   |
| ------ | ------------- | ----------------------------------------------------------- |
| `POST` | `/api/orders` | Cria um pedido completo (valida, calcula promoção e salva)  |
| `GET`  | `/api/orders/{id}` | Busca um pedido específico (para o cliente acompanhar) |
| `GET`  | `/api/orders/active` | Lista pedidos ativos (para o telão/display) |

### SignalR

| Rota             | Descrição                           |
| ---------------- | ----------------------------------- |
| `/hubs/orders`   | Hub para atualizações em tempo real |

---

## 📨 Criar Pedido — `POST /api/orders`

O frontend envia **todos os itens de uma vez**. O backend valida, calcula a promoção automaticamente e retorna o pedido completo.

### Request

```json
{
  "menuItemIds": [1, 4, 5]
}
```

### Response — Sucesso (201)

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "code": "#A42",
  "items": [
    { "menuItemId": 1, "name": "X Burger", "price": 5.00, "type": "Sandwich" },
    { "menuItemId": 4, "name": "Batata Frita", "price": 2.00, "type": "Side" },
    { "menuItemId": 5, "name": "Refrigerante", "price": 2.50, "type": "Drink" }
  ],
  "subtotal": 9.50,
  "promotionName": "Combo Completo",
  "discountPercent": 20,
  "discountValue": 1.90,
  "total": 7.60,
  "status": "Received",
  "createdAt": "2026-04-24T13:30:00Z"
}
```

### Response — Erro (400)

```json
{
  "code": "DUPLICATE_SANDWICH",
  "message": "Só é permitido 1 sanduíche por pedido."
}
```

---

## ❌ Códigos de Erro

Erros de domínio retornam `HTTP 400` com `code` e `message` para o frontend tratar.

| Código              | Mensagem                                  | Quando                    |
| ------------------- | ----------------------------------------- | ------------------------- |
| `DUPLICATE_SANDWICH`  | Só é permitido 1 sanduíche por pedido.  | 2+ sanduíches no pedido   |
| `DUPLICATE_SIDE`      | Só é permitido 1 acompanhamento por pedido. | 2+ acompanhamentos    |
| `DUPLICATE_DRINK`     | Só é permitida 1 bebida por pedido.     | 2+ bebidas no pedido      |
| `EMPTY_ORDER`         | O pedido deve conter pelo menos 1 item. | Nenhum item enviado       |
| `INVALID_MENU_ITEM`   | Item do cardápio não encontrado: {id}.  | ID inexistente no cardápio |

### Uso no React

```typescript
try {
  const response = await api.post('/api/orders', { menuItemIds: selectedIds });
  // sucesso — response.data contém o pedido
} catch (err) {
  const { code, message } = err.response.data;

  switch (code) {
    case 'DUPLICATE_SANDWICH':
    case 'DUPLICATE_SIDE':
    case 'DUPLICATE_DRINK':
      toast.error(message);
      break;
    case 'EMPTY_ORDER':
      toast.warn(message);
      break;
    case 'INVALID_MENU_ITEM':
      toast.error(message);
      break;
  }
}
```

---

## 🔔 SignalR — Tempo Real

O hub `/hubs/orders` emite eventos para manter o frontend sincronizado.

| Evento               | Descrição                                       |
| -------------------- | ----------------------------------------------- |
| `NewOrderReceived`   | Novo pedido chegou (notifica admin)             |
| `OrderStatusChanged` | Pedido mudou de status (notifica display/cliente) |

### Grupos

| Grupo         | Quem recebe                          |
| ------------- | ------------------------------------ |
| `admin`       | Painel do restaurante                |
| `display`     | Tela geral de acompanhamento        |
| `order-{id}`  | Cliente acompanhando pedido específico |

---

## 🛠️ Stack

### Backend

- **C# / .NET 10** — ASP.NET Core Web API
- **SignalR** — comunicação em tempo real
- **Swagger / OpenAPI** — documentação da API
- **Arquitetura em 4 camadas** (Domain, Application, Infrastructure, API)

### Frontend

- **React** (SPA)
- **SignalR Client** — tempo real

### Banco de Dados

- **In-Memory** (fase atual — dados estáticos)
- **SQL Server** (futuro — EF Core + Migrations)

---

## 📁 Estrutura do Projeto

```
📦 good-hamburger/
├── 📂 backend/
│   ├── 📂 GoodHamburger.API/             → Controllers, Hubs, Middlewares
│   ├── 📂 GoodHamburger.Application/     → Services, DTOs, Interfaces
│   ├── 📂 GoodHamburger.Domain/          → Entities, Enums, Errors, Regras
│   ├── 📂 GoodHamburger.Infrastructure/  → Repositories, Data (seed)
│   ├── 📂 GoodHamburger.Tests/           → Testes unitários
│   └── 📄 GoodHamburger.slnx
├── 📂 frontend/                          → React app (futuro)
├── 📄 .gitignore
└── 📄 README.md
```

---

## 🚀 Como Rodar

### Backend

```bash
# Restaurar dependências
dotnet restore ./backend

# Rodar a API
dotnet run --project ./backend/GoodHamburger.API
```

> A API estará disponível em `http://localhost:5205`.

### 📖 Documentação Interativa (Scalar UI)

No .NET 10, utilizamos o **Scalar** para documentação. Ele substitui o Swagger tradicional com uma interface mais moderna.

- **Acesse em:** `http://localhost:5205/scalar/v1`

---

## 🔐 Autenticação e Rotas Protegidas

As rotas administrativas requerem um token JWT.

### 1. Obter Token (Login)
Faça um `POST` para `/api/auth/login` com as credenciais admin. Você receberá um campo `token`.

### 2. Usar o Token
Para acessar rotas como `GET /api/admin/orders`, você deve enviar o token no cabeçalho (header) da requisição:

```http
Authorization: Bearer <SEU_TOKEN_AQUI>
```

---

## 🔐 Credenciais Admin (Seed)

Para acessar as rotas de gerenciamento (Kanban), o restaurante deve fazer login.

| Endpoint | Método | Auth | Descrição |
|---|---|---|---|
| `/api/auth/login` | `POST` | ❌ | Email e senha para obter o JWT token |

**Credenciais padrão (Seed):**
- **Email:** `admin@goodhamburger.com`
- **Senha:** `admin123`

---

## 📦 Gerenciamento de Pedidos (Admin)

| Endpoint | Método | Auth | Descrição |
|---|---|---|---|
| `/api/admin/orders` | `GET` | 🔒 | Lista todos os pedidos (para o Kanban) |
| `/api/admin/orders/{id}/status` | `PATCH` | 🔒 | Altera o status do pedido |

### 🚦 Fluxo de Status e Regras

Os pedidos seguem um fluxo rigoroso de transição:

1. **Received** (Recebido) → `Preparing` ou `Cancelled`
2. **Preparing** (Em preparo) → `Ready` ou `Cancelled`
3. **Ready** (Pronto) → `Delivered`
4. **Delivered** (Entregue) → Finalizado
5. **Cancelled** (Cancelado) → Finalizado

> [!WARNING]
> **Validação:** O backend lançará `INVALID_STATUS_TRANSITION` se tentar pular etapas ou voltar status (ex: `Preparing` → `Received`).

---

## 📡 SignalR (Real-time)

O backend utiliza SignalR para atualizações em tempo real no hub `/hubs/orders`.

### Grupos e Eventos

| Grupo | Evento | Payload | Descrição |
|---|---|---|---|
| `admin` | `NewOrderReceived` | `OrderResponse` | Notifica o painel admin de um novo pedido |
| `admin` | `OrderStatusChanged` | `{ orderId, code, oldStatus, newStatus }` | Atualiza o status no Kanban |
| `display` | `OrderStatusChanged` | `{ orderId, code, oldStatus, newStatus }` | Atualiza o telão de senhas |
| `order-{id}` | `OrderStatusChanged` | `{ orderId, code, newStatus }` | Notifica o cliente do pedido específico |

---

## ❌ Códigos de Erro (Adicionais)

| Código | Mensagem |
|---|---|
| `ORDER_NOT_FOUND` | Pedido não encontrado. |
| `INVALID_STATUS_TRANSITION` | Transição de status inválida. |
| `INVALID_CREDENTIALS` | Email ou senha incorretos. |

---

## 📝 Observações

- O cardápio é **fixo** (dados estáticos).
- O **pagamento é simulado**.
- O **cliente não precisa de login** (experiência de totem).
- A promoção é calculada **automaticamente**.
- O fluxo de restaurante é protegido por **JWT**.
