import Vue from 'vue';
import Router from 'vue-router';

import Home from '../components/Home.vue';
import RegisterTeam from '../components/RegisterTeam.vue';
import Lobby from '../components/Lobby.vue';
// import Login from '../components/Login.vue';

Vue.use(Router);

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Home',
      component: Home
    },
    {
      path: '/register',
      name: 'Register',
      props: true,
      component: RegisterTeam
    },
    {
      path: '/lobby',
      name: 'Lobby',
      component: Lobby
    }
  ]
});
