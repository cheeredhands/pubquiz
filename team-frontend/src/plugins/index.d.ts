import Vue, { VueConstructor } from 'vue';
import { AxiosInstance } from 'axios';
import { SnotifyService } from 'vue-snotify/SnotifyService';
import { QuizrHelpers } from './quizr-helpers';

declare global {
  interface Window {
    axios: AxiosInstance;
  }
}

declare module 'vue/types/vue' {
  interface Vue {
    $axios: AxiosInstance;
    $snotify: SnotifyService;
    $quizrhelpers: QuizrHelpers;
  }
  interface VueConstructor {
    $axios: AxiosInstance;
    $snotify: SnotifyService;
    $quizrhelpers: QuizrHelpers;
  }
}
