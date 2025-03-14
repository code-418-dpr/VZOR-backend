FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/VZOR.Web/VZOR.Web.csproj", "src/VZOR.Web/"]
COPY ["src/Accounts/VZOR.Accounts.Controllers/VZOR.Accounts.Controllers.csproj", "src/Accounts/VZOR.Accounts.Controllers/"]
COPY ["src/Accounts/VZOR.Accounts.Application/VZOR.Accounts.Application.csproj", "src/Accounts/VZOR.Accounts.Application/"]
COPY ["src/Shared/VZOR.Core/VZOR.Core.csproj", "src/Shared/VZOR.Core/"]
COPY ["src/Shared/VZOR.SharedKernel/VZOR.SharedKernel.csproj", "src/Shared/VZOR.SharedKernel/"]
COPY ["src/Accounts/VZOR.Accounts.Contracts/VZOR.Accounts.Contracts.csproj", "src/Accounts/VZOR.Accounts.Contracts/"]
COPY ["src/Accounts/VZOR.Accounts.Domain/VZOR.Accounts.Domain.csproj", "src/Accounts/VZOR.Accounts.Domain/"]
COPY ["src/Accounts/VZOR.Accounts.Infrastructure/VZOR.Accounts.Infrastructure.csproj", "src/Accounts/VZOR.Accounts.Infrastructure/"]
COPY ["src/Shared/VZOR.Framework/VZOR.Framework.csproj", "src/Shared/VZOR.Framework/"]
COPY ["src/Images/VZOR.Images.Application/VZOR.Images.Application.csproj", "src/Images/VZOR.Images.Application/"]
COPY ["src/Images/VZOR.Images.Domain/VZOR.Images.Domain.csproj", "src/Images/VZOR.Images.Domain/"]
COPY ["src/Shared/VZOR.gRPC/VZOR.gRPC.csproj", "src/Shared/VZOR.gRPC/"]
COPY ["src/Images/VZOR.Images.Controllers/VZOR.Images.Controllers.csproj", "src/Images/VZOR.Images.Controllers/"]
COPY ["src/Images/VZOR.Images.Contracts/VZOR.Images.Contracts.csproj", "src/Images/VZOR.Images.Contracts/"]
COPY ["src/Images/VZOR.Images.Infrastructure/VZOR.Images.Infrastructure.csproj", "src/Images/VZOR.Images.Infrastructure/"]
RUN dotnet restore "src/VZOR.Web/VZOR.Web.csproj"
COPY . .
WORKDIR "/src/src/VZOR.Web"
RUN dotnet build "VZOR.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "VZOR.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VZOR.Web.dll"]
