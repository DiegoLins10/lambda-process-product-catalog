provider "aws" {
  region = var.region
}

# ----------------------------
#  ZIP da Lambda (.NET 8)
# ----------------------------
data "archive_file" "lambda_zip" {
  type        = "zip"
  source_dir  = "${path.module}/../app/src/ProductCatalogLambda/bin/Release/net8.0"
  output_path = "${path.module}/lambda.zip"
}