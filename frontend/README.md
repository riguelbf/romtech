# Frontend App

This is the frontend application for our inventory management system, built using React, TypeScript, and Tailwind CSS.

## Table of Contents

- [Frontend App](#frontend-app)
  - [Table of Contents](#table-of-contents)
  - [Getting Started](#getting-started)
  - [Project Structure](#project-structure)
  - [Available Scripts](#available-scripts)
  - [Components](#components)
  - [API Documentation](#api-documentation)

## Getting Started

To get started with the frontend app, follow these steps:

1. Install the dependencies by running `pnpm install`.
2. Start the development server by running `pnpm dev`.
3. Open your web browser and navigate to `http://localhost:5173` to view the app.

## Project Structure

The frontend app is organized into the following directories:

* `components`: Reusable UI components, such as `Button`, `ProductModal`, and `ProductDataTable`.
* `features`: Feature-specific directories, such as `products`, which contains components and APIs related to product management.
* `pages`: Top-level pages for the app, such as `Home`.
* `public`: Static assets and index.html file.
* `styles`: Global CSS styles, including Tailwind CSS configurations.
* `utils`: Utility functions and helpers.

## Available Scripts

In the project directory, you can run the following scripts:

* `pnpm dev`: Starts the development server.
* `pnpm build`: Builds the app for production.
* `pnpm preview`: Starts a production server to preview the app.

## Components

The frontend app uses the following components:

* `Button`: A reusable button component.
* `ProductModal`: A modal component for displaying product information.
* `ProductDataTable`: A data table component for displaying product data.
* `Loader`: A loading indicator component.

## API Documentation

The frontend app communicates with the backend API, which is documented in the [./backend README.md file](../backend/README.md). The API endpoints are defined in the `features/api` directory, and are used to fetch and update product data.