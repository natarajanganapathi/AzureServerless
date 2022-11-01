

## Reference

1. [DevOps Ref](https://www.programmingwithwolfgang.com/deploy-azure-functions-azure-devops-pipelines/)

2. [Function Filter Ref](https://www.c-sharpcorner.com/article/do-you-know-azure-function-have-function-filters/#:~:text=Exception%20Filter%20will%20be%20called%20whenever%20there%20is,the%20Class%20level%20or%20the%20Function%20level.%20Prerequisites)

3. [call-another-function-with-in-an-azure-function](https://stackoverflow.com/questions/46315734/how-to-call-another-function-with-in-an-azure-function)

## Run Azure Function 4.0 in local dev environment

```
func host start
```

## Access the Azure fucntion

```
https://<azure-function>/api/participant-by-id?code=<client code>
```

## Swagger URL

[AzConf API Swagger URL](https://<azure-function>/api/swagger/ui#/)

## Known Issues

### Question 1
```
func : File C:\Users\Natarajan_Ganapathi\AppData\Roaming\npm\func.ps1 cannot be loaded. The file 
C:\Users\Natarajan_Ganapathi\AppData\Roaming\npm\func.ps1 is not digitally signed. You cannot run this 
script on the current system. For more information about running scripts and setting execution policy, 
see about_Execution_Policies at https:/go.microsoft.com/fwlink/?LinkID=135170.
At line:1 char:1
+ func host start
+ ~~~~
    + CategoryInfo          : SecurityError: (:) [], PSSecurityException
    + FullyQualifiedErrorId : UnauthorizedAccess
```
### Answer

```
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser

// Other Commands
Get-ExecutionPolicy -list
Set-ExecutionPolicy AllSigned -Scope CurrentUser
```

### Question 2
```
MongodDB Orderby or short not working 
```
### Answer

```
Create index for shorting fields.

// Composite Index in Azure Cosmos DB

use <DatabaseName>;
db.<CollectionName>.createIndex({Score:1,TimeTaken:1})
db.<CollectionName>.getIndexes() 
db.<CollectionName>.dropIndex(<filter>);
ex:
use AzConf;
db.LeaderBoard.createIndex({Score:1,TimeTaken:-1});
db.LeaderBoard.getIndexes() 
db.LeaderBoard.dropIndex({})
```

## local.settings.json

```json
{
    "Values": {
        "OpenApi__Info__Version": "v1",
        "OpenApi__Info__Title": "My API",
        "OpenApi__Info__Description": "My API description",
        "OpenApi__Info__TermsOfService": "https://example.com/terms",
        "OpenApi__Info__Contact_Name": "Natarajan Ganapathi",
        "OpenApi__Info__Contact_Email": "natarajanmca11@outlook.com",
        "OpenApi__Info__Contact_Url": "https://example.com/contact", 
        "OpenApi__Info__License_Name": "MIT",
        "OpenApi__Info__License_Url": "https://example.com/license"
    }
}
    ```