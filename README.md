# 🚀 ProductCatalogLambda – AWS Lambda para Processamento de Catálogo de Produtos

![.NET](https://img.shields.io/badge/.NET-8-blue?logo=dotnet)
![AWS Lambda](https://img.shields.io/badge/AWS%20Lambda-Managed-orange?logo=amazonaws)
![DynamoDB](https://img.shields.io/badge/DynamoDB-NoSQL-green?logo=amazondynamodb)
![S3](https://img.shields.io/badge/S3-Storage-yellow?logo=amazons3)
![Coverage](https://img.shields.io/badge/Coverage-0%25-lightgrey)

Lambda em **.NET 8** que processa arquivos `.xlsx` enviados por fornecedores, atualizando produtos em **DynamoDB** via **upsert**, acionada por eventos de **S3** através do **EventBridge**.

---

## 📖 Visão Geral

Este projeto automatiza a ingestão de catálogos de produtos, garantindo que novos produtos sejam inseridos e produtos existentes atualizados no DynamoDB, de forma escalável e serverless.

### Fluxo de Trabalho

1. Fornecedor envia `.xlsx` para o bucket **S3** `catalogo-produtos-uploads`.
2. Evento `ObjectCreated` é enviado para **EventBridge**.
3. **Lambda ProductCatalogLambda** é acionada.
4. Lambda baixa o arquivo, lê produtos via `XlsxHelper` e faz **upsert** no **DynamoDB**.

---

## 📂 Estrutura do Projeto

```
ProductCatalogLambda/
├─ FunctionHandler.cs              # Entry point da Lambda 🚀
├─ Extensions/
│   └─ ServiceCollectionExtensions.cs  # Configuração de DI 🔧
├─ Models/
│   └─ Product.cs                  # POCO do produto 🏷️
├─ Services/
│   ├─ AWS/
│   │   ├─ Interfaces/             # IS3Service, IDynamoService 🛠️
│   │   └─ Implementations/        # S3Service, DynamoService 📦
│   └─ Implementations/
│       ├─ XlsxHelper.cs           # Leitura XLSX 📄
│       └─ ProductProcessingService.cs # Processamento ⚡
├─ ProductCatalogLambda.csproj
└─ aws-lambda-tools-defaults.json
```

---

## ⚙️ Dependências

| Dependência                                    | Propósito                           |
| ---------------------------------------------- | ----------------------------------- |
| **Amazon.Lambda.Core**                         | Serialização e contexto da Lambda   |
| **Amazon.Lambda.Serialization.SystemTextJson** | Serialização JSON de eventos        |
| **AWSSDK.S3**                                  | Acesso e download de arquivos do S3 |
| **AWSSDK.DynamoDBv2**                          | Operações de upsert no DynamoDB     |
| **ClosedXML**                                  | Leitura de arquivos `.xlsx`         |
| **Microsoft.Extensions.DependencyInjection**   | Injeção de dependência (DI)         |

---

## 🌐 Modelo de Dados

O arquivo `.xlsx` deve conter as colunas:

| Coluna      | Tipo    |
| ----------- | ------- |
| SKU         | string  |
| Name        | string  |
| Description | string  |
| Price       | decimal |
| Category    | string  |

---

## 🛠 Tecnologias

* 🟢 **.NET 8**
* 🌐 **AWS Lambda**
* 💾 **DynamoDB (NoSQL)**
* 📦 **S3**
* 📄 **ClosedXML**
* ⚙️ **Dependency Injection (DI)**

---

## 🚀 Como rodar localmente

1. Clone o repositório:

```bash
git clone https://github.com/SeuUsuario/lambda-process-product-catalog.git
cd lambda-process-product-catalog
```

2. Abra no **Visual Studio 2022** e restaure pacotes:

```bash
dotnet restore
dotnet build
```

3. Teste a Lambda com **AWS Lambda Test Tool** usando um evento simulado:

```json
{
  "detail": {
    "value": {
      "bucket_name": "catalogo-produtos-uploads",
      "file_name": "produtos_fornecedor_XYZ.xlsx"
    }
  }
}
```

4. Faça deploy para **AWS Lambda** pelo Visual Studio ou CLI da AWS.

---

## 📦 GitHub Actions – CI/CD

* Workflow sugerido para **build**, **teste unitário** e **deploy Lambda**.
* Cobertura de testes e análises podem ser adicionadas com `coverlet` e badges atualizados automaticamente.

---

## 🔧 Próximos Passos

* [ ] 🔄 Logging estruturado com **CloudWatch**
* [ ] 🔒 Configurar IAM Role mínima (S3 + DynamoDB)
* [ ] 🧪 Adicionar testes unitários e de integração
* [ ] 📑 Documentar eventos do EventBridge e XLSX
* [ ] 🔀 Melhorar tratamento de erros e validações de dados

---

## 💡 Observações

* Clean Architecture aplicada: Lambda, serviços AWS, helpers e modelos separados.
* DynamoDB realiza **upsert** natural via `PutItemAsync`.
* Arquivos `.xlsx` devem conter colunas corretas (`SKU`, `Name`, `Description`, `Price`, `Category`).

---

✨ **by Diego Lins**

---
