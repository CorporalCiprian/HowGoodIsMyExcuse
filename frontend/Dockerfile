# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY package.json package-lock.json ./
RUN npm ci

# Copy source code
COPY . .

# Build Angular app
RUN npm run build

# Runtime stage - serve with nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Copy nginx config
COPY nginx.conf /etc/nginx/conf.d/

# Copy built app from build stage
COPY --from=build /app/dist/frontend/browser .

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
