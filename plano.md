# Plano: Substituição HTTP POST → Azure Service Bus Queue

**Objetivo:** Substituir o envio HTTP de dados de sensores por escrita na fila `salvar-dados-sensor` via Azure Service Bus.
**Connection string:** Key Vault → `ConnectionStrings:AzureServiceBus`

---

## Status Geral

- [x] Implementação completa
- [ ] Testes validados

---

## Checklist de Implementação

### 1. NuGet Package
- [x] Adicionar `Azure.Messaging.ServiceBus` v7.18.4 ao `AgroSolutions.Infra.csproj`

### 2. Extensão IoC — Service Bus
- [x] Criar `src/AgroSolutions.Infra/Ioc/RegisterServiceBusExtensions.cs`
  - [x] Lê `ConnectionStrings:AzureServiceBus` de `IConfiguration` (resolvido do Key Vault em runtime)
  - [x] Lança `InvalidOperationException` se connection string não encontrada (fail-fast no startup)
  - [x] Registra `ServiceBusClient` como **Singleton**

### 3. Novo Repository
- [x] Criar `src/AgroSolutions.Infra/Data/Repositories/SendDataSensorServiceBusRepository.cs`
  - [x] Implementa `ISendDataSensorRepository` (interface inalterada)
  - [x] `const string QueueName = "salvar-dados-sensor"`
  - [x] Serializa `SendDataSensorDto` com `System.Text.Json`
  - [x] Define `ContentType = "application/json"`
  - [x] Define `CorrelationId = TalhaoId` para rastreamento
  - [x] Define `ApplicationProperties["TalhaoId"]` e `["DataAfericao"]`
  - [x] Log **Debug**: payload JSON completo (ativado via `host.json`)
  - [x] Log **Information**: `MessageId` após envio bem-sucedido
  - [x] Retorna `false` + log de erro em caso de exceção
  - [x] `await using var sender` — descarte correto do sender

### 4. Atualizar IoC Registration
- [x] `RegisterRepositoryExtensions.cs`: trocar `SendDataSensorRepository` → `SendDataSensorServiceBusRepository`

### 5. Program.cs
- [x] Adicionar `.AddServiceBus(builder.Configuration)` após `.AddAzureSecrets(...)` na cadeia de startup

### 6. local.settings.json
- [x] Adicionada chave `ConnectionStrings:AzureServiceBus` com valor vazio
  - ⚠️ Deixar vazio se o ambiente local usa Key Vault via `DefaultAzureCredential`
  - ⚠️ Preencher com connection string de dev se rodar sem Key Vault; **não commitar o valor**

---

## Checklist de Validação

- [ ] Executar `dotnet build` sem erros
- [ ] Executar localmente com `func start`
- [ ] Disparar o timer manualmente e confirmar no log:
  - [ ] `"Preparando envio para fila 'salvar-dados-sensor' | TalhaoId=... | DataAfericao=..."`
  - [ ] `"✓ Mensagem enviada para fila 'salvar-dados-sensor' | TalhaoId=... | MessageId=..."`
- [ ] Inspecionar a fila `salvar-dados-sensor` no Service Bus Explorer (Portal Azure):
  - [ ] Body JSON contém os 6 campos: `TalhaoId`, `Umidade`, `DataAfericao`, `Temperatura`, `IndiceUv`, `VelocidadeVento`
  - [ ] `ApplicationProperties` contém `TalhaoId` e `DataAfericao`
  - [ ] `CorrelationId` = `TalhaoId`
- [ ] Debug de payload completo:
  - [ ] Alterar `host.json` → `logging.logLevel.default` para `"Debug"` temporariamente
  - [ ] Confirmar log com payload JSON completo
  - [ ] Restaurar nível de log após validação

---

## Arquivos Modificados

| Arquivo | Ação |
|---|---|
| `AgroSolutions.Infra/AgroSolutions.Infra.csproj` | ✅ Adicionado NuGet `Azure.Messaging.ServiceBus` |
| `AgroSolutions.Infra/Ioc/RegisterServiceBusExtensions.cs` | ✅ Criado |
| `AgroSolutions.Infra/Data/Repositories/SendDataSensorServiceBusRepository.cs` | ✅ Criado |
| `AgroSolutions.Infra/Ioc/RegisterRepositoryExtensions.cs` | ✅ Atualizado |
| `AF_AgroSolutions.Sinalizador.Talhao/Program.cs` | ✅ Atualizado |
| `AF_AgroSolutions.Sinalizador.Talhao/local.settings.json` | ✅ Atualizado |

## Arquivos NÃO Alterados (por design)

| Arquivo | Motivo |
|---|---|
| `ISendDataSensorRepository.cs` | Interface preservada — zero impacto nas camadas superiores |
| `ISendDataSensorService.cs` | Interface preservada |
| `SendDataSensorService.cs` | Serviço preservado |
| `ProcessarService.cs` | Orquestrador preservado |
| `SendDataSensorRepository.cs` | Mantido intacto — permite rollback imediato trocando o registro no IoC |

---

## Rollback

Para reverter para o envio HTTP original, alterar apenas **uma linha** em `RegisterRepositoryExtensions.cs`:

```csharp
// De:
services.AddTransient<ISendDataSensorRepository, SendDataSensorServiceBusRepository>();
// Para:
services.AddTransient<ISendDataSensorRepository, SendDataSensorRepository>();
```
