import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    // always use default values to the state
    // as to trigger the default state changed detection system.
    quiz: {
      // defines the current team
      team: null,
      // defines the other teams
      teams: []
    }
  },
  getters: {

  },
  mutations: {
    setTeam(state, team) {
      // called when the current team registers succesfully
      state.quiz.team = team;
    },
    addTeam(state, team) {
      // called by the signalr stuff when a new team registers
      state.quiz.teams.push(team);
    }
  },
});
