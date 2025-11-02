import { AppConfig } from "../core/models/config";

export const environment: AppConfig = {
  production: false,
  apiHost: 'http://localhost:5001', // .NET API base URL
  apiEndpoint: 'api/v1' // Base API path
};