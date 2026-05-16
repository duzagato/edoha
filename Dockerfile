# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Cache de restore: copiar apenas arquivos de projeto antes do código-fonte
COPY src/Edoha.sln .
COPY src/Edoha.Application/Edoha.Application.csproj Edoha.Application/
COPY src/Edoha.Domain/Edoha.Domain.csproj Edoha.Domain/
COPY src/Edoha.Infraestructure/Edoha.Infraestructure.csproj Edoha.Infraestructure/
COPY src/Edoha.Shared/Edoha.Shared.csproj Edoha.Shared/
RUN dotnet restore Edoha.sln

# Copiar código e publicar
COPY src/ .
RUN dotnet publish Edoha.Application/Edoha.Application.csproj \
    -c Release -o /app/publish --no-restore

# Runtime stage — imagem base Lambda para .NET 8 (Container Image Lambda)
FROM public.ecr.aws/lambda/dotnet:8 AS runtime
COPY --from=build /app/publish ${LAMBDA_TASK_ROOT}

# Handler: nome do assembly (sem extensão)
CMD ["Edoha.Application"]
