FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /src

COPY *.sln .
COPY EasyCiteLib/*.csproj ./EasyCiteLib/
COPY EasyCite/*.csproj ./EasyCite/

RUN dotnet restore EasyCite

COPY . ./
RUN dotnet publish -c Release EasyCite -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine

WORKDIR /app

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
ENV LC_ALL en_US.UTF-8
ENV LANG en_US.UTF-8
RUN apk add --no-cache icu-libs

COPY --from=build /app ./

ENTRYPOINT ["dotnet", "EasyCite.dll"]
