import Vue from 'vue';
// import './plugins/axios';
import './plugins/quizr-helpers';
import App from './App.vue';
import router from './router/index';
import store from './store/index';
import Snotify, { SnotifyDefaults } from 'vue-snotify';
import BootstrapVue from 'bootstrap-vue';
import { library } from '@fortawesome/fontawesome-svg-core'
import { faSignOutAlt, faComment } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import axios from 'axios';

library.add(faSignOutAlt, faComment);
Vue.component('font-awesome-icon', FontAwesomeIcon);

Vue.config.productionTip = false;

// tslint:disable-next-line:no-object-literal-type-assertion
const options = {
  toast: {
    showProgressBar: false
  }
} as SnotifyDefaults;

Vue.use(Snotify, options);

Vue.use(BootstrapVue);

const instanceUserApi = axios.create({
  baseURL: 'https://localhost:5001/'
});
instanceUserApi.interceptors.request.use(
  cfg => {
    // Do something before request is sent
    if (localStorage.getItem('token')) {
      instanceUserApi.defaults.headers.common.Authorization = 'Bearer ' + localStorage.getItem('token');
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
  render: h => h(App)
}).$mount('#app');
