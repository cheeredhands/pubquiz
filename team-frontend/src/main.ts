import Vue from 'vue';
import './plugins/axios';
import App from './App.vue';
import router from './router/index';
import store from './store/index';
import Snotify, { SnotifyDefaults } from 'vue-snotify';

Vue.config.productionTip = false;

// tslint:disable-next-line:no-object-literal-type-assertion
const options = {
  toast: {
    showProgressBar: false
  }
} as SnotifyDefaults;

Vue.use(Snotify, options);

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app');
