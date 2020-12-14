import Vue from 'vue';
import './plugins/quizr-helpers';
import App from './App.vue';
import router from './router';
import store from './store';
import { BootstrapVue, BIconExclamationTriangle, BIconBoxArrowUpRight, BIconFileEarmarkPlus, BIconSearch } from 'bootstrap-vue';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faSignOutAlt, faTrashAlt, faPlay, faPause, faStop, faPowerOff, faUser, faArrowLeft, faArrowRight, faPen, faCheck, faGlasses, faExternalLinkSquareAlt, faCheckSquare, faCheckCircle, faTimesCircle, faPlus } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import axios from 'axios';
import i18n from './plugins/i18n';

library.add(faGlasses, faSignOutAlt, faTrashAlt, faPlay, faPause, faStop, faPowerOff, faUser, faArrowLeft, faArrowRight, faPen, faCheck, faExternalLinkSquareAlt, faCheckCircle, faTimesCircle, faPlus);
Vue.component('font-awesome-icon', FontAwesomeIcon);

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
