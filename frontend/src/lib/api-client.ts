import Axios, { InternalAxiosRequestConfig } from 'axios';
import { env } from '@/config/env';

let token: string | undefined = undefined;

export function initializeToken(newToken: string | undefined) {
  token = newToken;
}

async function authRequestInterceptor(config: InternalAxiosRequestConfig) {
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
});

api.interceptors.request.use(authRequestInterceptor);
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      console.log(error.response);
      if (token) {
        console.log('401 but Token is available');
      } else {
        console.error('401 and token is not available');
      }
    }

    return Promise.reject(error);
  },
);
