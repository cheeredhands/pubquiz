import Vue, { PluginObject } from 'vue';
import axios from 'axios';

// Full config:  https://github.com/axios/axios#request-config
// axios.defaults.baseURL = process.env.baseURL || process.env.apiUrl || '';
// axios.defaults.headers.common['Authorization'] = AUTH_TOKEN;
// axios.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded';

const config = {
  // baseURL: process.env.baseURL || process.env.apiUrl || ""
  // timeout: 60 * 1000, // Timeout
  // withCredentials: true, // Check cross-site Access-Control
};

const axiosInstance = axios.create(config);

axiosInstance.interceptors.request.use(
  cfg => {
    // Do something before request is sent
    return cfg;
  },
  err => {
    // Do something with request error
    return Promise.reject(err);
  }
);

// Add a response interceptor
axiosInstance.interceptors.response.use(
  res => {
    // Do something with response data
    return res;
  },
  err => {
    // Do something with response error
    return Promise.reject(err);
  }
);

const Plugin: PluginObject<any> = {
  install: Vue => {
    Vue.$axios = axiosInstance;
  }
};
Plugin.install = Vue => {
  Vue.$axios = axiosInstance;
  window.axios = axiosInstance;
  Object.defineProperties(Vue.prototype, {
    $axios: {
      get() {
        return axiosInstance;
      }
    }
  });
};

Vue.use(Plugin);

export default Plugin;
