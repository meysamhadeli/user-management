import { AppConfig } from "../core/models/config";

export const environment: AppConfig = {
  production: false,
  apiHost: 'https://localhost:5000', // .NET API base URL
  apiEndpoint: 'api/v1' // Base API path
};