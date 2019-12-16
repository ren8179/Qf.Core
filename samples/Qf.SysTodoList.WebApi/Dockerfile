#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["samples/Qf.SysTodoList.WebApi/Qf.SysTodoList.WebApi.csproj", "samples/Qf.SysTodoList.WebApi/"]
COPY ["src/Qf.Core/Qf.Core.csproj", "src/Qf.Core/"]
COPY ["src/Qf.Core.Web/Qf.Core.Web.csproj", "src/Qf.Core.Web/"]
RUN dotnet restore "samples/Qf.SysTodoList.WebApi/Qf.SysTodoList.WebApi.csproj"
COPY . .
WORKDIR "/src/samples/Qf.SysTodoList.WebApi"
RUN dotnet build "Qf.SysTodoList.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Qf.SysTodoList.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Qf.SysTodoList.WebApi.dll"]