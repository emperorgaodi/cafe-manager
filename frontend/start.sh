#!/bin/sh

# Write nginx config at startup using the BACKEND_URL environment variable.
# This runs every time the container starts, so Railway env vars are available.

cat > /etc/nginx/conf.d/default.conf << NGINX
server {
    listen 80;

    location / {
        root /usr/share/nginx/html;
        index index.html;
        try_files \$uri \$uri/ /index.html;
    }

    location /api/ {
        proxy_pass ${BACKEND_URL}/api/;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }
}
NGINX

echo "Starting nginx with BACKEND_URL=${BACKEND_URL}"
nginx -g "daemon off;"
