FROM mcr.microsoft.com/devcontainers/dotnet:9.0

RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs \
    

RUN npm install -g pnpm

WORKDIR /workspace
