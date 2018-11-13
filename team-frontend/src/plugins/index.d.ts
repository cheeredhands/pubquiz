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
  }
  interface VueConstructor {
    $axios: AxiosInstance;
  }
}

// https://github.com/artemsky/vue-snotify/issues/60
declare module 'vue/types/vue' {
  interface Vue {
    $snotify: SnotifyService;
  }
}
