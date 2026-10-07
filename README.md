# 📋ApiClinica

API REST para gerenciamento de uma clínica médica, desenvolvida para a disciplina de **Programação Server-Side** — Engenharia de Software, Centro Universitário Católica de Santa Catarina.

Projeto do Trabalho N2: evolução da ApiClinica para uso de **ORM (Entity Framework Core)** e **DTOs**.

## ✨Tecnologias

- C# / .NET 10
- Entity Framework Core
- Testes de endpoints via Postman

## ✨Estrutura do projeto

```
ApiClinica/
├── Controllers/      # PacientesController, MedicosController, ConsultasController
├── Models/            # Paciente, Medico, Consulta
├── Data/              # AppDbContext
├── DTOs/              # DTOs de Create, Read e Update para cada entidade
├── Mappers/           # PacienteMapper, MedicoMapper, ConsultaMapper (manuais)
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

## ✨Funcionalidades

Cada controller (`Pacientes`, `Medicos`, `Consultas`) expõe os métodos `GET`, `GET by Id`, `POST`, 'PUT'/`PATCH` e `DELETE`, recebendo e retornando apenas DTOs — nunca as entidades do banco diretamente.

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
- Não é possível haver dois horários iguais para o mesmo médico ou o mesmo paciente.
- Não pode haver sobreposição de horários (cada consulta dura 30 minutos).
- Ao alterar PacienteId ou MedicoId via PATCH, as regras de conflito de horário são revalidadas.

**Regras gerais**
- No PATCH, campos recebidos como `null` são ignorados (não atualizados).
- Não é possível excluir (DELETE) um paciente ou médico que tenha consultas futuras agendadas.
- Todas as operações utilizam o `AppDbContext` (EF Core).

## ✨Como executar

Pré-requisitos: [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Passos

```bash
# Clonar o repositório
git clone https://github.com/beatrice-fischer/ApiClinica.git
cd ApiClinica

# Restaurar dependências
dotnet restore

# Aplicar as migrations e criar o banco SQLite
dotnet ef database update

# Executar a API
dotnet run
```

A API sobe por padrão em `https://localhost:{porta}` (verifique a porta exibida no console ou em `Properties/launchSettings.json`).

### Testando os endpoints

Todos os endpoints podem ser testados via [Postman](https://www.postman.com/). Uma collection poderá ser adicionada futuramente na pasta do projeto.

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
