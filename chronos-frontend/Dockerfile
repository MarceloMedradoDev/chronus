# --- Etapa 1: Build da aplicação Angular ---
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build -- --configuration=production

# --- Etapa 2: Servir com Nginx ---
FROM nginx:alpine
# Copia o build gerado para a pasta pública do Nginx (ajuste o caminho 'chronos-frontend' para o nome do seu projeto no angular.json)
COPY --from=build /app/dist/chronos-frontend/browser /usr/share/nginx/html

# Copia uma configuração customizada do nginx se precisar (opcional)
# COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]