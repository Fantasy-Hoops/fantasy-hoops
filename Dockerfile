FROM microsoft/dotnet:sdk AS build
WORKDIR /app

RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs

COPY config.env .
COPY *.csproj .
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "fantasy_hoops.dll"]