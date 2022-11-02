# Azure Serverless
This is sample project developed in Azure Function v4 with dotnet 6. This repo is integrated with [Cloud Sonarqube](https://sonarcloud.io/summary/new_code?id=natarajanganapathi_AzureServerless) to validate the code quality and security check. 


## Reference

1. [Azure Function Ref.](https://github.com/MicrosoftDocs/azure-docs/blob/main/articles/azure-functions/create-first-function-cli-csharp.md)


## Todo:

### V.1.0

1. Azure Key Vault - Done
2. AppInsights Configuration 
3. .Net 7 standards Standards
4. Azure Standards
5. C# Lint Configuration 
6. Sonar Lint configuration 
7. Sonarqube Integration (Quality, Security) - Done
8. GitHub Actions (Trunk based) - Done
9. Sequence Diagram Documentation
10. Dependency Diagram documentation

### V.2.0

1. Event Driven / Async Function call
2. Durable Function

### Verificatoin:

1.  ASK, Container APPs, Container Ins, Azure Fun, App Service

### PAAS

1. Azure App Service Deployment using GitHub Actions
2. Send Email after complete the Test (Service Bus)

### local.settings.json

```json
{
    "Values": {
        "OpenApi__Info__Version": "v1",
        "OpenApi__Info__Title": "My API",
        "OpenApi__Info__Description": "My API description",
        "OpenApi__Info__TermsOfService": "https://example.com/terms",
        "OpenApi__Info__Contact__Name": "Natarajan Ganapathi",
        "OpenApi__Info__Contact__Email": "natarajanmca11@outlook.com",
        "OpenApi__Info__Contact__Url": "https://example.com/contact", 
        "OpenApi__Info__License__Name": "MIT",
        "OpenApi__Info__License__Url": "https://example.com/license"
    }
}
```
