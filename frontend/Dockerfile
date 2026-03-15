# ── Build stage ────────────────────────────────────────────────────────────────
FROM node:22-alpine AS build
WORKDIR /app

COPY package.json ./
RUN npm install

COPY . .
RUN npm run build

# ── Serve with nginx ───────────────────────────────────────────────────────────
FROM nginx:stable-alpine AS runtime

COPY --from=build /app/dist /usr/share/nginx/html

# Copy nginx config as a template — envsubst fills in BACKEND_URL at startup
COPY nginx.conf /etc/nginx/templates/default.conf.template

EXPOSE 80
