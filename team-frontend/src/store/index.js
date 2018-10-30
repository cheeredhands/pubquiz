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
    // mutations are sync store updates
    setTeam(state, team) {
      // called when the current team registers succesfully
      state.quiz.team = team;
    },
    addTeam(state, team) {
      // called by the signalr stuff when a new team registers
      state.quiz.teams.push(team);
    },
    setOwnTeamName(state, newName) {
      state.quiz.team.teamName = newName;
    },
    setOtherTeam(state, team) {
      console.log('setOtherTeam: ' + team); // eslint-disable-line no-console

      var teamInStore = state.quiz.teams.find(item => item.teamId === team.teamId);
      teamInStore.teamName = team.teamName; // throws error in console...
    },
    saveSignalRConnection(state, signalrconnection) {
      state.signalrconnection = signalrconnection;
    }
  },
  actions: {
    // actions are async store updates and use the commit method to delegate
    // the action to the mutation as actions are not allowed to change the state directly.
    initTeam({ commit }, team) {
      commit("setTeam", team);
      gamehub.init();
    },
    renameOtherTeam({ commit }, team) {
      console.log('renameOtherTeam: ' + team); // eslint-disable-line no-console
      commit('setOtherTeam', team);
    },
    processTeamRegistered({ commit }, teamRegistered) {
      const addedTeam = {
        teamId: teamRegistered.teamId,
        teamName: teamRegistered.teamName
      };
      if (addedTeam.teamId !== this.state.quiz.team.teamId) {
        // because the hub is not (yet) capable of notifying other teams
        // we receive our own teamRegistered notification as well.
        commit("addTeam", addedTeam);
      }
    }
  }
});
