# Use the official Node.js 20 image as a base
FROM node:20

# Set the working directory to /app
WORKDIR /app

# Copy the package*.json files to the working directory
COPY package*.json ./

# Install the dependencies
RUN pnpm install --frozen-lockfile

# Copy the application code to the working directory
COPY . .

# Build the application
RUN pnpm build

# Expose the port that the application will use
EXPOSE 5173

# Set the environment variable for Vite
ENV VITE_NODE_VERSION 20

# Run the command to start the development server
CMD ["pnpm", "dev"]