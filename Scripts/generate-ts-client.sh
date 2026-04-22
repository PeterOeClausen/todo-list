#!/bin/bash

# To install the nswag CLI tool, run: 
# dotnet tool install --global NSwag.ConsoleCore

# Fetch open api spec:
curl https://localhost:5000/openapi/v1.json > swagger.json;

# Generate api client:
nswag openapi2tsclient /input:swagger.json /output:../Frontend/todo-app/src/clients/todo-api-client.ts /classname:TodoApiClient;