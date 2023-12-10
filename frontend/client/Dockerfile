# Stage 1: Build the Angular app in a Node.js environment
FROM node:20.10.0 as build

# Set the working directory
WORKDIR /app

# Copy package.json and package-lock.json
COPY package*.json ./

# Install dependencies, including Angular CLI
RUN npm install
RUN npm install -g @angular/cli

# Copy the rest of the Angular app source code
COPY . .

# Build the Angular app for production
RUN ng build --configuration production

# Stage 2: Set up Nginx to serve the Angular app
FROM nginx:alpine

# Copy the custom Nginx configuration file
COPY nginx.conf /etc/nginx/nginx.conf

# Copy the built files from the build stage to the Nginx server
COPY --from=build /app/dist/client/browser /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start Nginx when the container launches
CMD ["nginx", "-g", "daemon off;"]
