variable "region" {
  type        = string
  default     = "us-east-1"
}

variable "lambda_name" {
  type        = string
  default     = "lambda-process-product-catalog"
}

variable "project" {
  type        = string
  default     = "product-catalog"
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
