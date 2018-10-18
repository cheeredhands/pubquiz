import Vue from "vue";
import Vuex from "vuex";
import * as SignalR from "@aspnet/signalr";

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

      // init signalR
      // initialize signalR connection here
      const connection = new SignalR.HubConnectionBuilder()
        .withUrl("https://localhost:5001/gamehub")
        .build();
      connection.start().then(() => {
        // save it
        commit("saveSignalRConnection", connection);
      });
      // todo set up the server callbacks
      // connection.on("send", data => {
      //   console.log(data);
      // });

      // send something to the backend
      // connection.start().then(() => connection.invoke("send", "Hello"));
    }
  }
});
