import axios from "axios";
import Cookies from "js-cookie";

export const apiClient = axios.create({
  baseURL: "/api/proxy/v1",
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use(
  (config) => {
    const token = Cookies.get("accessToken");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      const refreshToken = Cookies.get("refreshToken");

      if (refreshToken) {
        try {
          const { data } = await axios.post("/api/proxy/v1/auth/refresh", { refreshToken });
          
          Cookies.set("accessToken", data.data.accessToken);
          Cookies.set("refreshToken", data.data.refreshToken);

          originalRequest.headers.Authorization = `Bearer ${data.data.accessToken}`;
          return apiClient(originalRequest);
        } catch (refreshError) {
          Cookies.remove("accessToken");
          Cookies.remove("refreshToken");
          Cookies.remove("user");
          window.location.href = "/login";
          return Promise.reject(refreshError);
        }
      }
    }

    return Promise.reject(error);
  }
);
