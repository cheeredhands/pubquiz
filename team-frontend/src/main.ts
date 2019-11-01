import Vue from 'vue';
// import './plugins/axios';
import './plugins/quizr-helpers';
import App from './App.vue';
import router from './router/index';
import store from './store/index';
import VueI18n from 'vue-i18n'

import BootstrapVue from 'bootstrap-vue';
import { library } from '@fortawesome/fontawesome-svg-core'
import { faSignOutAlt, faComment } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import i18n from './plugins/i18n'
import axios from 'axios';

library.add(faSignOutAlt, faComment);
Vue.component('font-awesome-icon', FontAwesomeIcon);

Vue.config.productionTip = false;

Vue.use(BootstrapVue, {
  BToast: {
    toaster: 'b-toaster-bottom-right'
  }
});

const instanceUserApi = axios.create({
  baseURL: 'https://localhost:5001/'
});
instanceUserApi.interceptors.request.use(
  cfg => {
    // Do something before request is sent
    if (localStorage.getItem('token')) {
      cfg.headers.common.Authorization = 'Bearer ' + localStorage.getItem('token');
    }
    // else {
    //   instanceUserApi.defaults.headers.common.Authorization = undefined;
    // }
    return cfg;
  },
  err => {
    // Do something with request error
    return Promise.reject(err);
  }
);


Vue.prototype.$axios = instanceUserApi;

new Vue({
  router,
  store,
  i18n,
  render: h => h(App)
}).$mount('#app');
