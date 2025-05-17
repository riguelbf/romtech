# Use a imagem oficial do Node.js 20 como base
FROM node:20

# Defina o diretório de trabalho
WORKDIR /app

# Instale o pnpm usando Corepack
RUN corepack enable && corepack prepare pnpm@latest --activate

# Copie apenas arquivos de dependência primeiro para aproveitar o cache
COPY app/package.json app/pnpm-lock.yaml ./

# Instale dependências
RUN pnpm install --frozen-lockfile

# Copie o restante do projeto
COPY ./app .

# Construa a aplicação para produção
RUN pnpm build

# Exponha a porta padrão do Vite
EXPOSE 5173

# Configure o ambiente se necessário
ENV VITE_NODE_VERSION=20

# Comando padrão: iniciar o Vite em modo dev (ou use preview para produção)
CMD ["pnpm", "dev", "--host"]
