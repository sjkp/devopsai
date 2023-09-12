# devopsai
A tool to showcase how Open AI function calling can be utilized in various devops tasks. Currently the tool implements 
function for looking up information from nuget and for looking up githup repository data.

## Example 1 

```
devopsai ask -sys "" -msg "For the following nuget packages please find their license   > Ardalis.ListStartupServices                               1.1.3       1.1.3 \n > MediatR.Extensions.Microsoft.DependencyInjection          10.0.1      10.0.1"
```
**Result**
```
1. Ardalis.ListStartupServices: MIT License, GitHub [Link](https://github.com/ardalis/AspNetCoreStartupServices)
2. MediatR.Extensions.Microsoft.DependencyInjection: Apache-2.0, Nuget License [Link](https://licenses.nuget.org/Apache-2.0)
```

## Example 2

```
devopsai ask -sys "" -msg "For the following nuget packages please list the author as markdown table   > Ardalis.ListStartupServices                               1.1.3       1.1.3 \n > MediatR.Extensions.Microsoft.DependencyInjection          10.0.1      10.0.1"
```
**Result:**

| Nuget Package                                      | Author                 |
|---------------------------------------------------|------------------------|
| Ardalis.ListStartupServices (1.1.3)               | Steve Smith (@ardalis) |
| MediatR.Extensions.Microsoft.DependencyInjection (10.0.1) | Jimmy Bogard          |

```
devopsai ask -sys "" -msg "Which of the following packages have the most github stars MediatR Moq"
```
**Result** as of 12th of september 2023
```
The MediatR package, hosted by user "jbogard" on GitHub, has 9902 stars. On the other hand, the Moq package, hosted by the "moq" organization on GitHub, has 5388 stars. Therefore, the MediatR package has more Github stars.
```

