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

## Check elestic is running

curl http://localhost:9200/

You should see a response as:
```json
{
  name: "33cb86340ca8",
  cluster_name: "docker-cluster",
  cluster_uuid: "5oXs7pKjR9Kp539wa7My9g",
  version: {
    number: "7.6.2",
    build_flavor: "default",
    build_type: "docker",
    build_hash: "ef48eb35cf30adf4db14086e8aabd07ef6fb113f",
    build_date: "2020-03-26T06:34:37.794943Z",
    build_snapshot: false,
    lucene_version: "8.4.0",
    minimum_wire_compatibility_version: "6.8.0",
    minimum_index_compatibility_version: "6.0.0-beta1"
  },
  tagline: "You Know, for Search"
}
```

## Connect to UIs with your browser

Kibana: http://localhost:5601

RabbitMQ: http://localhost:15672 (user:guest,pwd:guest)

## Send a message to RabbitMQ
```code
curl -u guest:guest -X POST -H "Content-Type:application/json" -d '{"properties":{"content-type": "application/json"},"routing_key":"#.#.#","payload":"{\"M 
essage\":\"hello world\"}","payload_encoding":"string"}' http://localhost:15672/api/exchanges/%2F/logging.application.serilog/publish
{"routed":true}
```

## Check in Kibana 
Create an index with the pattern : logstash-*

You will then be able to see messages coming from RabbitMQ under: http://localhost:5601/app/kibana#/discover


## Build Standalone WebApi docker images

```code
cd WebApi
docker build -t webapi .
docker run --name webapi  -p 8080:80  webapi
```
