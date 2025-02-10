import Axios, { InternalAxiosRequestConfig } from 'axios';
import { env } from '@/config/env';

// Helper function to store and retrieve the token securely
const TOKEN_KEY = "authToken";

export function initializeToken(newToken: string | undefined) {
  if (newToken) {
    sessionStorage.setItem(TOKEN_KEY, newToken); // Store token securely
  } else {
    sessionStorage.removeItem(TOKEN_KEY); // Clear token on logout
  }
}

async function authRequestInterceptor(config: InternalAxiosRequestConfig) {
  const token = sessionStorage.getItem(TOKEN_KEY); // Retrieve token from sessionStorage

  if (config.headers) {
    config.headers.Accept = 'application/json';
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }

  config.withCredentials = true;
  return config;
}

export const api = Axios.create({
  baseURL: env.API_URL,
  withCredentials: true
});

api.interceptors.request.use(authRequestInterceptor);
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      console.log(error.response);
      
      const token = sessionStorage.getItem(TOKEN_KEY); // Check token from sessionStorage
      if (token) {
        console.log('401 but Token is available');
      } else {
        console.error('401 and token is not available');
      }
    }

    return Promise.reject(error);
  },
);