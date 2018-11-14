import Vue, { VueConstructor } from 'vue';
import { AxiosInstance } from 'axios';
import { SnotifyService } from 'vue-snotify/SnotifyService';

declare global {
  interface Window {
    axios: AxiosInstance;
  }
}

declare module 'vue/types/vue' {
  interface Vue {
    $axios: AxiosInstance;
    $snotify: SnotifyService;
  }
  interface VueConstructor {
    $axios: AxiosInstance;
  }
}

