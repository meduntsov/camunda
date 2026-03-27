FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/WorkflowMvp.Api/WorkflowMvp.Api.csproj src/WorkflowMvp.Api/
RUN dotnet restore src/WorkflowMvp.Api/WorkflowMvp.Api.csproj
COPY src/WorkflowMvp.Api src/WorkflowMvp.Api
RUN dotnet publish src/WorkflowMvp.Api/WorkflowMvp.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "WorkflowMvp.Api.dll"]
