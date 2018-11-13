import Vue from 'vue';
import Vuex, { StoreOptions } from 'vuex';
import './plugins/axios';
import App from './App.vue';
import router from './router/index';
import store from './store/index';
import Snotify, { SnotifyToastConfig } from 'vue-snotify';

Vue.config.productionTip = false;

// Vue.use(Snotify, (options: SnotifyToastConfig) => {
//   options.showProgressBar = false;
// });

Vue.use(Snotify);

Vue.use(Vuex);

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app');
