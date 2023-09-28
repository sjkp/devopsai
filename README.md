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


## Example 3 (Azure DevOps)
```
devopsai ask -sys "" -msg "which azure devops workitems are referenced to the build with id 358" -project GPTReviewTest
```
**Result**
```
AI: The build with ID 358 references a workitem with the ID 15. Here is the information about the workitem:

- Area Path: GPTReviewTest
- Team Project: GPTReviewTest
- Iteration Path: GPTReviewTest
- WorkItem Type: Bug
- State: New
- Created Date: 2023-09-14T20:56:10.433Z
- Created By: Simon Pedersen (blasimtex@hotmail.com)
- Changed Date: 2023-09-14T21:00:26.527Z
- Changed By: Simon Pedersen (blasimtex@hotmail.com)
- Priority: 1 (High)
- Title: "this is a test"
- Reproduction Steps: "Once upon a midnight dreary, while I pondered, weak and weary, over many a quaint and curious volume of forgotten lore - while I nodded, nearly napping, suddenly there came a tapping, as of someone gently rapping, rapping at my chamber door. 'Tis some visitor, I muttered, tapping at my chamber door; - only this and nothing more."

You can find more details about this workitem by visiting this [link](https://dev.azure.com/sjkp/276ff40e-aa6c-499b-aec0-cf52989e4d7a/_workitems/edit/15).
```


# Release
`dotnet pack -p:configuration=release -p:PackageVersion=1.0.2`
