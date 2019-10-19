import Vue, { VueConstructor } from 'vue';
import { AxiosInstance } from 'axios';
import { QuizrHelpers } from './quizr-helpers';

declare global {
  interface Window {
    axios: AxiosInstance;
  }
}

declare module 'vue/types/vue' {
  interface Vue {
    $axios: AxiosInstance;
    $quizrhelpers: QuizrHelpers;
  }
  interface VueConstructor {
    $axios: AxiosInstance;
    $quizrhelpers: QuizrHelpers;
  }
}
