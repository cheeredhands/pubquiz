import Vue from 'vue';
import Router from 'vue-router';

import Home from '../components/Home.vue';
import RegisterTeam from '../components/RegisterTeam.vue';
import TeamLobby from '../components/TeamLobby.vue';
import QuizMasterLogin from '../components/QuizMasterLogin.vue';
import QuizMasterLobby from '../components/QuizMasterLobby.vue';
import QuizMasterGame from '../components/QuizMasterGame.vue';

Vue.use(Router);

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Home',
      component: Home
    },
    {
      path: '/registerteam',
      name: 'RegisterTeam',
      props: true,
      component: RegisterTeam
    },
    {
      path: '/lobby',
      name: 'TeamLobby',
      component: TeamLobby
    },
    {
      path: '/qm/login',
      name: 'QuizMasterLogin',
      component: QuizMasterLogin
    },
    {
      path: '/qm/lobby',
      name: 'QuizMasterLobby',
      component: QuizMasterLobby
    },
    {
      path: '/qm/game',
      name: 'QuizMasterGame',
      component: QuizMasterGame
    }
  ]
});
