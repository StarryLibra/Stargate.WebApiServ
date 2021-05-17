#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#Official images for .NET and ASP.NET Core: https://hub.docker.com/_/microsoft-dotnet/

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Stargate.WebApiServ.Web/Stargate.WebApiServ.Web.csproj", "Stargate.WebApiServ.Web/"]
RUN dotnet restore "Stargate.WebApiServ.Web/Stargate.WebApiServ.Web.csproj"
COPY . .
WORKDIR "/src/Stargate.WebApiServ.Web"
RUN dotnet build "Stargate.WebApiServ.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stargate.WebApiServ.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stargate.WebApiServ.Web.dll"]