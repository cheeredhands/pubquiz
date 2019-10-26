import Vue from 'vue';
import './plugins/axios';
import './plugins/quizr-helpers';
import App from './App.vue';
import router from './router/index';
import store from './store/index';
import VueI18n from 'vue-i18n'
import BootstrapVue from 'bootstrap-vue';
import { library } from '@fortawesome/fontawesome-svg-core'
import { faSignOutAlt, faComment } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

library.add(faSignOutAlt, faComment);
Vue.component('font-awesome-icon', FontAwesomeIcon);

Vue.config.productionTip = false;

Vue.use(BootstrapVue, VueI18n, {
  BToast: {
    toaster: 'b-toaster-bottom-right'
  }
});

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app');
