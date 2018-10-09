import Vue from 'vue';
import Router from 'vue-router';

Vue.use(Router);

import Home from '../components/Home.vue';
import RegisterTeam from '../components/RegisterTeam.vue';
import Lobby from '../components/Lobby.vue';

export default new Router({
  routes:[{
    path: '/',
    name: 'Home',
    component: Home
  },{
    path: '/register',
    name: 'Regisyter',
    component: RegisterTeam
  },{
    path: '/lobby',
    name: 'Lobby',
    component: Lobby
  }]
});
