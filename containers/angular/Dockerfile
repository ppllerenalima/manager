## --------------------------
## Etapa 1: Compilar Angular
## --------------------------
#FROM node:18 AS build   # Usa la imagen oficial de Node.js v18 como base y la nombra "build"
#WORKDIR /app            # Define el directorio de trabajo dentro del contenedor en /app
#
## Copia solo los archivos de dependencias para aprovechar la caché de Docker
#COPY packages/main/package*.json ./   # Copia package.json y package-lock.json al contenedor
#RUN npm install                        # Instala las dependencias de Angular dentro del contenedor
#
## Copia el resto del proyecto Angular
#COPY packages/main/ ./                 # Copia todo el código fuente (src, angular.json, etc.) al contenedor
#RUN npm run build --prod               # Ejecuta el build de Angular en modo producción (salida en dist/)
#
## --------------------------
## Etapa 2: Servir con Nginx
## --------------------------
#FROM nginx:alpine                      # Usa la imagen oficial de Nginx ligera (Alpine)
#COPY --from=build /app/dist/managersire /usr/share/nginx/html  
## Copia los archivos generados en la etapa de build (dist/managersire) a la carpeta que Nginx sirve por defecto
#
#EXPOSE 80                              # Expone el puerto 80 del contenedor (HTTP)
#CMD ["nginx", "-g", "daemon off;"]     # Arranca Nginx en primer plano (necesario para que el contenedor no se apague)
#


#----------------------

# --------------------------
# Etapa 1: Compilar Angular
# --------------------------
FROM node:18 AS build
WORKDIR /app

# Copia dependencias
COPY package*.json ./        # ya no pongas "packages/main"
RUN npm install

# Copia el resto del código Angular
COPY . .                     # copia src, angular.json, tsconfig, etc.
RUN npm run build --prod

# --------------------------
# Etapa 2: Servir con Nginx
# --------------------------
FROM nginx:alpine
COPY --from=build /app/dist/managersire /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]