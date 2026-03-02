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
├─ Aws/                     # Clientes de serviços AWS (ex: S3, DynamoDB) ☁️
├─ Extensions/              # Métodos de extensão (ex: Injeção de Dependência) 🔧
├─ Helpers/                 # Classes utilitárias 🛠️
├─ Interfaces/              # Contratos das abstrações (ex: IProductProcessingService) 📜
├─ Models/                  # POCOs e DTOs (ex: Product) 🏷️
├─ Services/                # Lógica de negócio ⚡
├─ aws-lambda-tools-defaults.json # Configurações de deploy da Lambda ⚙️
├─ Function.cs              # Entry point da Lambda 🚀
└─ Readme.md                # Este arquivo 📄

ProductCatalogLambda.Unit.Tests/
├─ Aws/                     # Testes de serviços AWS 🧪
├─ Helpers/                 # Helpers de teste 🛠️
├─ Services/                # Testes da lógica de negócio 🧪
└─ FunctionTests.cs         # Testes do handler da Lambda 🎯
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

Quando um arquivo é adicionado ao bucket `catalogo-produtos-uploads`, o EventBridge envia um evento assim:

```json
{
  "version": "0",
  "id": "12345678-1234-1234-1234-123456789012",
  "detail-type": "Object Created",
  "source": "aws.s3",
  "account": "123456789012",
  "time": "2025-10-25T23:59:00Z",
  "region": "sa-east-1",
  "resources": [
    "arn:aws:s3:::catalogo-produtos-uploads"
  ],
  "detail": {
    "bucket": {
      "name": "catalogo-produtos-uploads"
    },
    "object": {
      "key": "produtos_fornecedor_XYZ.xlsx",
      "size": 1024,
      "eTag": "abc123def456ghi789",
      "sequencer": "0055AED6DCD90281E5"
    }
  }
}
```
> A Lambda `ProductCatalogLambda` vai extrair `bucket.name` e `object.key` desse payload para processar o arquivo XLSX.

4. Faça deploy para **AWS Lambda** pelo Visual Studio ou CLI da AWS.

---

## **Sessão de comandos S3**

### **1️⃣ Criar o bucket**

```bash
aws s3 mb s3://catalogo-produtos-uploads --region sa-east-1
```

* Cria o bucket na região `sa-east-1`.
* Nome deve ser **único globalmente**.

---

### **2️⃣ Verificar se o bucket foi criado**

```bash
aws s3 ls
```

* Lista todos os buckets da sua conta.
* Você deve ver `catalogo-produtos-uploads` na lista.

---

### **3️⃣ Dar permissão total para teste local (opcional)**

> Atenção: isso é apenas para teste local. Em produção, restrinja ao Lambda/usuário específico.

```bash
aws s3api put-bucket-policy --bucket catalogo-produtos-uploads --policy '{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": { "AWS": "*" },
      "Action": "s3:*",
      "Resource": [
        "arn:aws:s3:::catalogo-produtos-uploads",
        "arn:aws:s3:::catalogo-produtos-uploads/*"
      ]
    }
  ]
}'
```

---

### **4️⃣ Testar acesso ao bucket**

```bash
aws s3 ls s3://catalogo-produtos-uploads
```

* Deve listar objetos do bucket (vazio se recém-criado).
* Se funcionar → seu Lambda local **pode acessar o bucket**.

---

### **5️⃣ Enviar um arquivo de teste (opcional)**

```bash
aws s3 cp arquivo-teste.txt s3://catalogo-produtos-uploads/
```

* Envia `arquivo-teste.txt` para o bucket.
* Depois, verifique:

```bash
aws s3 ls s3://catalogo-produtos-uploads
```

* Você deve ver `arquivo-teste.txt`.

---


## 📦 GitHub Actions – CI/CD

* Workflow sugerido para **build**, **teste unitário** e **deploy Lambda**.
* Cobertura de testes e análises podem ser adicionadas com `coverlet` e badges atualizados automaticamente.

---

## 🔧 Próximos Passos

* [x] 🔄 Logging estruturado com **CloudWatch**
* [x] 🔒 Configurar IAM Role mínima (S3 + DynamoDB)
* [x] 🧪 Adicionar testes unitários e de integração
* [x] 📑 Criar IAC para a Lambda
* [x] 🔀 Melhorar tratamento de erros e validações de dados

---

## 💡 Observações

* Clean Architecture aplicada: Lambda, serviços AWS, helpers e modelos separados.
* DynamoDB realiza **upsert** natural via `PutItemAsync`.
* Arquivos `.xlsx` devem conter colunas corretas (`SKU`, `Name`, `Description`, `Price`, `Category`).

---

✨ **by Diego Lins**

---
