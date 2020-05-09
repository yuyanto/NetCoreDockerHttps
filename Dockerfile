#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=123456
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=conf.d/https/dev_cert.pfx
ENV SQLSERVER_HOST=db

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["NuGet.config", ""]
COPY devextreme /src/devextreme

COPY ["WhoGetWhat.csproj", ""]
RUN dotnet restore "./WhoGetWhat.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "WhoGetWhat.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WhoGetWhat.csproj" -c Release -o /app/publish

RUN dotnet user-secrets set "Authentication:Google:ClientId" "720837966232-gskksqi02tu6ojulnl49erm0c4p4cfsa.apps.googleusercontent.com"
RUN dotnet user-secrets set "Authentication:Google:ClientSecret" "J2T0EQ5Yt_gJSLOGQO6XHBvY"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY . /app
#RUN chmod +x ./entrypoint.sh
#CMD /bin/bash ./entrypoint.sh

ENTRYPOINT ["dotnet", "WhoGetWhat.dll"]