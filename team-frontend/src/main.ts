import Vue from 'vue';
import './plugins/axios';
import './plugins/quizr-helpers';
import App from './App.vue';
import router from './router/index';
import store from './store/index';
import BootstrapVue from 'bootstrap-vue';
import { library } from '@fortawesome/fontawesome-svg-core'
import { faSignOutAlt, faComment } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { ToastPlugin } from 'bootstrap-vue'

library.add(faSignOutAlt, faComment);
Vue.component('font-awesome-icon', FontAwesomeIcon);

Vue.config.productionTip = false;

Vue.use(BootstrapVue);
Vue.use(ToastPlugin)

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app');
