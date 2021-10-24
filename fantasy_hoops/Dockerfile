FROM mcr.microsoft.com/dotnet/sdk:5.0-focal-amd64 AS build 
WORKDIR /app

RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs

COPY *.csproj .
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "fantasy_hoops.dll"]
