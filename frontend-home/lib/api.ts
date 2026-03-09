import axios from 'axios';
import Cookies from 'js-cookie';

const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5153", // API Gateway base URL
});

// Attach access token to outgoing requests
api.interceptors.request.use((config) => {
  const token = Cookies.get('access_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// On 401, try token refresh once
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const accessToken = Cookies.get('access_token');
        const refreshToken = Cookies.get('refresh_token');
        const res = await axios.post("http://localhost:5153/api/Auth/refresh-token", {
          accessToken,
          refreshToken
        });

        if (res.status === 200) {
          Cookies.set('access_token', res.data.accessToken);
          Cookies.set('refresh_token', res.data.refreshToken);

          originalRequest.headers.Authorization = `Bearer ${res.data.accessToken}`;
          return api(originalRequest);
        }
      } catch (refreshError) {
        Cookies.remove('access_token');
        Cookies.remove('refresh_token');
        window.location.href = "/login";
      }
    }
    return Promise.reject(error);
  }
);

export default api;