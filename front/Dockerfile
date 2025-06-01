# ---------- Stage 1 : Build ----------
FROM node:18-slim AS builder
WORKDIR /app
    
COPY package*.json ./
RUN npm ci            # o npm install
    
COPY . .
ARG VITE_API_BASE=http://localhost:5000/api
ENV VITE_API_BASE=$VITE_API_BASE
    
RUN npm run build-only
    
# ---------- Stage 2 ----------
FROM nginx:alpine
COPY --from=builder /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]