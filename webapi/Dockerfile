FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
#COPY *.sln .
COPY webapi/*.csproj ./webapi/
RUN dotnet restore

# copy everything else and build app
COPY webapi/. ./webapi/
WORKDIR /app/webapi
RUN dotnet publish -c Release -o out

RUN Echo "17" > version.txt


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/webapi/out ./
ENTRYPOINT ["dotnet", "InitApp.dll"]
