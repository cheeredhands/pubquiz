import Vue from "vue";
import Vuex from "vuex";
import gamehub from "../services/gamehub";
//import * as SignalR from "@aspnet/signalr";

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
    },
    signalrconnection: null
  },
  getters: {},
  mutations: {
    setTeam(state, team) {
      // called when the current team registers succesfully
      state.quiz.team = team;
    },
    addTeam(state, team) {
      // called by the signalr stuff when a new team registers
      state.quiz.teams.push(team);
    },
    saveSignalRConnection(state, signalrconnection) {
      state.signalrconnection = signalrconnection;
    }
  },
  actions: {
    initTeam({ commit }, team) {
      commit("setTeam", team);

      // todo init gamehub
      gamehub.init();
    },
    processTeamRegistered({ commit }, teamRegistered) {
      const addedTeam = {
        teamId: teamRegistered.teamId,
        name: teamRegistered.teamName
      };
      commit("addTeam", addedTeam);
    }
  }
});
