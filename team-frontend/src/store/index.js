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
    initTeam({ dispatch, commit }, team) {
      commit("setTeam", team);

      // init signalR
      // initialize signalR connection here
      const connection = new SignalR.HubConnectionBuilder()
        .withUrl("https://localhost:5001/gamehub")
        .configureLogging(SignalR.LogLevel.Information)
        .build();
      connect(connection).then(() => {
        // save it
        commit("saveSignalRConnection", connection);
      });

      function connect(conn) {
        return conn.start().catch(e => {
          sleep(5000);
          console.log("Reconnecting Socket because of " + e); // eslint-disable-line no-console
          connect(conn);
        });
      }

      connection.onclose(function() {
        connect(connection);
      });

      function sleep(milliseconds) {
        var start = new Date().getTime();
        for (var i = 0; i < 1e7; i++) {
          if (new Date().getTime() - start > milliseconds) {
            break;
          }
        }
      }
      // todo set up the server callbacks
      // connection.on("send", data => {
      //   console.log(data);
      // });
      connection.on("TeamRegistered", data =>
        dispatch("processTeamRegistered", data)
      );

      // send something to the backend
      // connection.start().then(() => connection.invoke("send", "Hello"));
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
