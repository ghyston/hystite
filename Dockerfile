FROM mcr.microsoft.com/dotnet/core/sdk:5.0 AS build
WORKDIR /app

COPY Hysite.Web/hysite.csproj ./
RUN dotnet restore

COPY Hysite.Web ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:5.0-bionic

ARG HYSITE_VERSION="latest"

ENV HYSITE_VERSION=$HYSITE_VERSION

WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
EXPOSE 80

ENTRYPOINT ["dotnet", "hysite.dll"]