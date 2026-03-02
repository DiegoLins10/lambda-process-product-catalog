variable "region" {
  type    = string
  default = "us-east-1"
}

variable "lambda_name" {
  type    = string
  default = "lambda-process-product-catalog"
}

variable "project" {
  type    = string
  default = "product-catalog"
}

variable "env" {
  type        = map(string)
  description = "Variáveis de ambiente da Lambda"
  default     = {}
}

variable "allow_invoke_from_apigw" {
  type    = bool
  default = false
}

variable "s3_bucket_name" {
  type        = string
  description = "Nome do bucket S3 de origem dos arquivos"
  default     = "catalogo-produtos-uploads"
}

variable "dynamodb_table_name" {
  type        = string
  description = "Nome da tabela DynamoDB de produtos"
  default     = "ProductCatalog"
}
