import Vue from 'vue';
import Router from 'vue-router';

Vue.use(Router);

import RegisterTeam from '../components/RegisterTeam.vue';
import Lobby from '../components/Lobby.vue';

export default new Router({
  routes:[{
    path: '/',
    name: 'Home',
    component: RegisterTeam,
    children: [{
      path: 'lobby',
      name: 'Lobby',
      component: Lobby
    }]
  }]
});
