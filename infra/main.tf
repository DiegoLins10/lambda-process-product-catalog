# ----------------------------
# IAM Role da Lambda
# ----------------------------
resource "aws_iam_role" "lambda_role" {
  name = "${var.project}-lambda-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect = "Allow"
      Principal = {
        Service = "lambda.amazonaws.com"
      }
      Action = "sts:AssumeRole"
    }]
  })
}

# Permissões básicas de logs
resource "aws_iam_role_policy_attachment" "lambda_logs" {
  role       = aws_iam_role.lambda_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

# ----------------------------
# CloudWatch Log Group
# ----------------------------
resource "aws_cloudwatch_log_group" "lambda_lg" {
  name              = "/aws/lambda/${var.lambda_name}"
  retention_in_days = 30
}

# ----------------------------
# Lambda
# ----------------------------
resource "aws_lambda_function" "lambda" {
  function_name = var.lambda_name
  role          = aws_iam_role.lambda_role.arn

  handler = "ProductCatalogLambda::ProductCatalogLambda.Function::FunctionHandler"
  runtime = "dotnet8"

  filename         = data.archive_file.lambda_zip.output_path
  source_code_hash = data.archive_file.lambda_zip.output_base64sha256

  timeout = 30
  memory_size = 1024

  environment {
    variables = var.env
  }
}
