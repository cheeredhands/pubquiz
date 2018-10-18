import Vue from "vue";
import "./plugins/axios";
import App from "./App.vue";
import router from "./router/index";
//import signalr from "@aspnet/signalr";
import store from "./store/index.js";
import gamehub from './services/gamehub';

Vue.config.productionTip = false;

gamehub.init();

new Vue({
  render: h => h(App),
  router,
  store
}).$mount("#app");
