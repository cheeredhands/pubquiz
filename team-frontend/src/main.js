import Vue from 'vue'
import './plugins/axios'
import App from './App.vue'

Vue.config.productionTip = false

import router from './router/index';

new Vue({
  render: h => h(App),
  router
}).$mount('#app')
