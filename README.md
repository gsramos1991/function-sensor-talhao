# ⚡ AF AgroSolutions - Sinalizador de Talhão

Projeto **AF AgroSolutions**, uma Azure Function robusta projetada para o monitoramento e sinalização de dados de talhões agrícolas. 🌾🚜

Esta aplicação é parte integrante do ecossistema AgroSolutions, focada em automação, coleta de dados e observabilidade de alta performance.

## 🚀 Sobre o Projeto

A **Sinalizador.Talhao** é uma Azure Function disparada por Timer (Timer Trigger) que executa o ciclo completo de:
1.  **Autenticação:** Login seguro em serviços externos.
2.  **Coleta:** Obtenção de dados atualizados de talhões e propriedades.
3.  **Enriquecimento:** Integração com dados meteorológicos e temporais.
4.  **Processamento:** Envio de dados processados para repositórios e barramentos de mensagens (Service Bus).

## 🛠️ Tecnologias Utilizadas

-   **Runtime:** [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) 🖥️
-   **Serverless:** [Azure Functions Worker v1](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library) ⚡
-   **Mensageria:** [Azure Service Bus](https://azure.microsoft.com/services/service-bus/) 📩
-   **Segurança:** [Azure Key Vault](https://azure.microsoft.com/services/key-vault/) 🔐
-   **Observabilidade:** 📊
    -   **OpenTelemetry:** Coleta de métricas e traços.
    -   **Prometheus & Grafana:** Visualização de dados.
    -   **Loki:** Agregação de logs.
    -   **Tempo:** Rastreamento distribuído.

## 📂 Estrutura de Pastas

```text
azure-functions/
├── .github/                # Workflows de CI/CD (GitHub Actions)
├── observability/          # Configurações de monitoramento (Grafana, Loki, Prometheus, OTel)
│   └── grafana/            # Provisionamento de dashboards e datasources
├── src/                    # Código fonte do projeto
│   ├── AF_AgroSolutions.Sinalizador.Talhao/  # Projeto principal da Azure Function
│   ├── AgroSolutions.Busines/                # Lógica de negócio e interfaces
│   ├── AgroSolutions.Domain/                 # Entidades e modelos de domínio
│   └── AgroSolutions.Infra/                  # Integrações externas e repositórios
├── tests/                  # Testes unitários e de integração
├── Dockerfile              # Configuração para containerização
└── AF_AgroSolutions.Sinalizador.Talhao.slnx  # Solução do projeto
```

## 🏗️ Como Executar

1.  Certifique-se de ter o [.NET 8 SDK](https://dotnet.microsoft.com/download) instalado.
2.  Configure as variáveis de ambiente necessárias no `local.settings.json`.
3.  Execute o comando:
    ```bash
    func start
    ```

---

## 💻 Idealizadores do projeto (Discord name)
- 👨‍💻Clovis Alceu Cassaro (cloves_93258)
- 👨‍💻Gabriel Santos Ramos (_gsramos)
- 👨‍💻Júlio César de Carvalho (cesarsoft)
- 👨‍💻Marco Antonio Araujo (_marcoaz)
- 👩‍💻Yasmim Muniz Da Silva Caraça (yasmimcaraca)
