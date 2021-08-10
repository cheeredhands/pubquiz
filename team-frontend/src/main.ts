import Vue from 'vue';
import './plugins/quizr-helpers';
import App from './App.vue';
import router from './router';
import store from './store';
import { BootstrapVue, BIconExclamationTriangle, BIconBoxArrowUpRight, BIconFileEarmarkPlus, BIconSearch, BIconEyeglasses, BIconTrashFill, BIconPencilFill, BIconPlayFill, BIconPauseFill, BIconPower, BIconDoorOpenFill, BIconArrowLeftShort, BIconArrowLeft, BIconArrowRightShort, BIconArrowRight, BIconWindow, BIconCheckCircleFill, BIconXCircleFill, BIconCheckCircle, BIconCaretRightFill, BIconCaretDownFill } from 'bootstrap-vue';
import axios from 'axios';
import i18n from './plugins/i18n';

Vue.config.productionTip = false;

Vue.use(BootstrapVue, {
  BToast: {
    toaster: 'b-toaster-bottom-right'
  }
});
Vue.component('b-icon-exclamation-triangle', BIconExclamationTriangle);
Vue.component('b-icon-arrow-up-right', BIconBoxArrowUpRight);
Vue.component('b-icon-file-earmark-plus', BIconFileEarmarkPlus);
Vue.component('b-icon-search', BIconSearch);
Vue.component('b-icon-eyeglasses', BIconEyeglasses);
Vue.component('b-icon-trash-fill', BIconTrashFill);
Vue.component('b-icon-pencil-fill', BIconPencilFill);
Vue.component('b-icon-play-fill', BIconPlayFill);
Vue.component('b-icon-pause-fill', BIconPauseFill);
Vue.component('b-icon-power', BIconPower);
Vue.component('b-icon-door-open-fill', BIconDoorOpenFill);
Vue.component('b-icon-arrow-left-short', BIconArrowLeftShort);
Vue.component('b-icon-arrow-left', BIconArrowLeft);
Vue.component('b-icon-arrow-right-short', BIconArrowRightShort);
Vue.component('b-icon-arrow-right', BIconArrowRight);
Vue.component('b-icon-window', BIconWindow);
Vue.component('b-icon-check-circle', BIconCheckCircle);
Vue.component('b-icon-check-circle-fill', BIconCheckCircleFill);
Vue.component('b-icon-x-circle-fill', BIconXCircleFill);
Vue.component('b-icon-caret-right-fill', BIconCaretRightFill);
Vue.component('b-icon-caret-down-fill', BIconCaretDownFill);

// https://dev.to/heftyhead/lets-talk-about-an-unnecessary-but-popular-vue-plugin-1ied
const axiosInstanceBackend = axios.create({
  baseURL: process.env.VUE_APP_BACKEND_URI
});
axiosInstanceBackend.interceptors.request.use(
  cfg => {
    if (localStorage.getItem('token')) {
      cfg.headers.common.Authorization = 'Bearer ' + localStorage.getItem('token');
    }
    return cfg;
  },
  err => {
    // Do something with request error
    return Promise.reject(err);
  }
);

Vue.prototype.$axios = axiosInstanceBackend;
Vue.prototype.$http = axios;

Vue.filter('formatSize', function(size: number) {
  if (size > 1024 * 1024 * 1024 * 1024) {
    return (size / 1024 / 1024 / 1024 / 1024).toFixed(2) + ' TB';
  } else if (size > 1024 * 1024 * 1024) {
    return (size / 1024 / 1024 / 1024).toFixed(2) + ' GB';
  } else if (size > 1024 * 1024) {
    return (size / 1024 / 1024).toFixed(2) + ' MB';
  } else if (size > 1024) {
    return (size / 1024).toFixed(2) + ' KB';
  }
  return size.toString() + ' B';
});

new Vue({
  router,
  store,
  i18n,
  render: h => h(App)
}).$mount('#app');
