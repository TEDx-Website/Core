// import axios, { AxiosError, InternalAxiosRequestConfig } from 'axios';
// import { ApiResponse, ApiErrorResponse, AuthTokens } from '@/types/api';

// const getAccessToken = (): string | null => {
//   return null;
// };

// const getRefreshToken = (): string | null => {
//   return null;
// };

// const setTokens = (tokens: AuthTokens) => {
// };

// export const api = axios.create({
//   baseURL: process.env.NEXT_PUBLIC_API_URL,
//   headers: {
//     'Content-Type': 'application/json',
//   },
// });

// api.interceptors.request.use(
//   (config: InternalAxiosRequestConfig) => {
//     const token = getAccessToken();
//     if (token && config.headers) {
//       config.headers.Authorization = `Bearer ${token}`;
//     }
//     return config;
//   },
//   (error) => Promise.reject(error)
// );

// api.interceptors.response.use(
//   (response) => {
//     return response.data;
//   },
//   async (error: AxiosError<ApiResponse<null>>) => {
//     const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

//     if (error.response?.status === 401 && originalRequest && !originalRequest._retry) {
//       originalRequest._retry = true;

//       try {
//         const refreshToken = getRefreshToken();
        
//         if (!refreshToken) {
//           throw new Error();
//         }

//         const refreshResponse = await axios.post<ApiResponse<AuthTokens>>(
//           `${process.env.NEXT_PUBLIC_API_URL}/auth/refresh`,
//           { refreshToken }
//         );

//         const newTokens = refreshResponse.data.data;
        
//         setTokens(newTokens);

//         if (originalRequest.headers) {
//           originalRequest.headers.Authorization = `Bearer ${newTokens.accessToken}`;
//         }
        
//         return api(originalRequest);
//       } catch (refreshError) {
//         window.location.href = '/login';
//         return Promise.reject(refreshError);
//       }
//     }

//     const backendError: ApiErrorResponse | undefined = error.response?.data?.error;
//     return Promise.reject(backendError || error);
//   }
// );