import Vue from 'vue';
import Router from 'vue-router';

import Home from '../components/Home.vue';
import About from '../components/About.vue';
import TeamLobby from '../components/TeamLobby.vue';
import QuizMasterLogin from '../components/QuizMasterLogin.vue';
import QuizMasterLobby from '../components/QuizMasterLobby.vue';
import QuizMasterInGame from '../components/QuizMasterInGame.vue';
import Beamer from '../components/Beamer.vue';
import TeamInGame from '../components/TeamInGame.vue';

Vue.use(Router);

export default new Router({
  routes: [
    {
      path: '/',
      meta: {
        title: 'Welkom bij Quizr!'
      },
      name: 'Home',
      component: Home
    },
    {
      path: '/about',
      name: 'About',
      component: About
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
      name: 'QuizMasterInGame',
      component: QuizMasterInGame
    },
    {
      path: '/beamer',
      name: 'Beamer',
      component: Beamer
    },
    {
      path: '/game',
      name: 'TeamInGame',
      component: TeamInGame
    }
  ]
});
