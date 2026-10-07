# 📋ApiClinica

API REST para gerenciamento de uma clínica médica, desenvolvida para a disciplina de **Programação Server-Side** — Engenharia de Software, Centro Universitário Católica de Santa Catarina.

Projeto do Trabalho N2: evolução da ApiClinica para uso de **ORM (Entity Framework Core)** e **DTOs**.

## ✨Tecnologias

- C# / .NET 10
- Entity Framework Core (provider SQLite)
- Testes de endpoints via Postman

## ✨Estrutura do projeto

```
ApiClinica/
├── Controllers/      # PacientesController, MedicosController, ConsultasController
├── Models/            # Paciente, Medico, Consulta
├── Data/              # AppDbContext
├── DTOs/              # DTOs de Create, Read e Update para cada entidade
├── Mappers/           # PacienteMapper, MedicoMapper, ConsultaMapper (manuais)
├── Validators/        # CpfValidator, ContatoValidator (regras reutilizáveis)
├── Migrations/        # Migrations do EF Core
├── Program.cs
└── appsettings.json
```

## ✨Entidades

**Paciente**
- Id, Nome, Email, Telefone, DataNasc, Cpf

**Médico**
- Id, Nome, Email, Telefone, CRM

**Consulta**
- Id, PacienteId, MedicoId, DataHora

## ✨Camadas

O fluxo de uma requisição atravessa quatro camadas, e nenhuma delas invade a outra:

- **DTO** define o contrato público — o que entra e o que sai pela rede.
- **Mapper** traduz entre DTO e entidade. Cada entidade tem `ToEntity`, `ToReadDTO` e `ApplyUpdate`.
- **Validator** concentra as regras reutilizáveis, usadas por mais de um controller.
- **Controller** recebe o HTTP, valida, chama o Mapper, fala com o `AppDbContext` e escolhe o status da resposta.

A entidade nunca chega ao cliente e o DTO nunca chega ao banco — o Mapper é a única fronteira entre os dois.

## ✨Funcionalidades

Cada controller (`Pacientes`, `Medicos`, `Consultas`) expõe os métodos `GET`, `GET by Id`, `POST`, `PATCH` e `DELETE`, recebendo e retornando apenas DTOs — nunca as entidades do banco diretamente.

### Validações compartilhadas

Regras usadas por mais de um controller ficam em `Validators/`, como classes estáticas:

- **`CpfValidator.EhValido(cpf)`** — confere os dois dígitos verificadores e rejeita CPFs com todos os dígitos iguais. Aceita o CPF com ou sem pontuação.
- **`ContatoValidator.ValidarEmail(email)`** e **`ContatoValidator.ValidarTelefone(telefone)`** — devolvem `null` quando o valor é válido, ou a mensagem de erro quando não é.

### Regras de validação

**Paciente**
- Email em formato válido.
- Telefone no formato `(47) 98888-7777`.
- Data de nascimento não pode ser no futuro.
- CPF numericamente válido (não pode ser alterado via PATCH).
- Não é possível cadastrar um paciente com CPF já existente.

**Médico**
- Email em formato válido.
- Telefone no formato `(47) 98888-7777`.

**Consulta**
- PacienteId e MedicoId devem existir.
- Não é possível agendar consulta no passado.
- Não pode haver sobreposição de horários para o mesmo médico nem para o mesmo paciente: cada consulta ocupa 30 minutos, então duas consultas precisam ter ao menos esse intervalo entre si. Horários que apenas encostam (10:00 e 10:30) são permitidos.
- Ao alterar PacienteId ou MedicoId via PATCH, as regras de conflito de horário são revalidadas com os valores finais.

**Regras gerais**
- No PATCH, campos recebidos como `null` são ignorados (não atualizados).
- Não é possível excluir (DELETE) um paciente ou médico que tenha consultas futuras agendadas.
- Todas as operações utilizam o `AppDbContext` (EF Core).

### Códigos de status

| Código | Quando                                                          |
|-----------|---------------------------------------------------------|
| 200       | Consulta ou atualização bem-sucedida            |
| 201       | Recurso criado (com cabeçalho `Location`)       |
| 204       | Exclusão bem-sucedida                                  |
| 400       | Corpo da requisição inválido                           |
| 404       | Recurso não encontrado                                 |
| 409       | Conflito com o estado atual (CPF duplicado, horário ocupado, exclusão bloqueada) |

## ✨Como executar

Pré-requisitos: [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Passos

```bash
# Clonar o repositório
git clone https://github.com/beatrice-fischer/ApiClinica.git
cd ApiClinica

# Restaurar dependências
dotnet restore

# Executar a API
dotnet run
```

O banco SQLite é criado e as migrations são aplicadas automaticamente na inicialização — o `Program.cs` chama `Database.Migrate()`, então não é necessário rodar `dotnet ef database update` manualmente.

O arquivo `clinica.db` fica na raiz do projeto, versionado no repositório, para que toda a equipe trabalhe sobre a mesma base. O caminho é calculado a partir da pasta do `.csproj`, não do diretório de trabalho, de modo que a API encontra o mesmo banco seja executada pelo Visual Studio, por `dotnet run` ou pelo executável.

A API sobe em `http://localhost:{porta}` — confira a porta na linha `Now listening on:` exibida no console ao iniciar.

### Testando os endpoints

A collection do Postman está na raiz do projeto: **`ApiClinica-N2.postman_collection.json`**.

Importe em **Import → File** e execute pelo **Runner**, de cima para baixo — a ordem importa, porque os IDs criados nas primeiras requisições alimentam as seguintes. Ajuste a variável `baseUrl` para a porta exibida no console.

A collection cobre os cinco métodos nas três entidades, os casos de erro de cada validação, a janela de 30 minutos e a regressão das rotas no singular. Ela gera CPF válido e datas futuras a cada execução e remove ao final tudo o que criou.

Exemplos de rotas disponíveis:

| Método  | Rota                             | Descrição                             |
|-----------|---------------------------|---------------------------------|
| GET       | `/api/pacientes`         | Lista todos os pacientes      |
| GET       | `/api/pacientes/{id}`  | Busca paciente por Id         |
| POST     | `/api/pacientes`         | Cadastra um novo paciente |
| PATCH   | `/api/pacientes/{id}` | Atualiza um paciente           |
| DELETE | `/api/pacientes/{id}` | Remove um paciente            |

(Rotas análogas para `/api/medicos` e `/api/consultas`.)

## ✨Equipe

- Beatrice Fischer
- Gabriele Maria Freiberger
- Gustavo Hreczuck
- Lucas de Carvalho Ziele
- Raul Schmitz

## ✨Disciplina

- **Curso:** Engenharia de Software
- **Disciplina:** Programação Server-Side
- **Professora:** Beatriz M. Reichert
- **Instituição:** Centro Universitário Católica de Santa Catarina
