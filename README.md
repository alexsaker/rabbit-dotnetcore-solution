# Dotnet Core Api using ElasticSearch and Kibana for log vsualisation

## Create solution

```code
dotnet new sln -o RabbitDotnetcoreSolution
```

## Create webapi

```code
dotnet new webapi -o ./RabbitDotnetcoreSolution/WebApi
```

## Link webapi to solution

```code
dotnet sln RabbitDotnetcoreSolution/RabbitDotnetcoreSolution.sln add RabbitDotnetcoreSolution/WebApi/WebApi.csproj
```

## Add necessary packages

```code
cd RabbitDotnetcoreSolution/WebApi

dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Enrichers.Environment
dotnet add package Serilog.Sinks.Debug
dotnet add package Serilog.Sinks.Elasticsearch
dotnet add package Serilog.Exceptions

dotnet restore
```

## Change appsettings.json to get the logs to kibana

```code
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Information",
        "System": "Warning"
      }
    }
  },
  "ElasticConfiguration": {
    "Uri": "http://localhost:9200"
  },
  "AllowedHosts": "*"
}
```

## Run all the services with docker-compose

```code
docker-compose up -d
```

## Build Standalone WebApi docker images

```code
cd WebApi
docker build -t webapi .
docker run --name webapi  -p 8080:80  webapi
```
