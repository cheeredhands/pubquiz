import Vue from "vue";
import Vuex, { StoreOptions } from "vuex";
import gamehub from "../services/gamehub";
import { Quiz, TeamInfo } from "@/models/models";
import { HubConnection } from "@aspnet/signalr";

Vue.use(Vuex);

export interface RootState {
  quiz: Quiz;
  signalrconnection?: HubConnection;
}

const store: StoreOptions<RootState> = {
  state: {
    quiz: { team: undefined, teams: [] },
    signalrconnection: undefined
  },
  getters: {},
  mutations: {
    // mutations are sync store updates
    setTeam(state, team: TeamInfo) {
      // called when the current team registers succesfully
      state.quiz.team = team;
    },
    addTeam(state, team: TeamInfo) {
      // called by the signalr stuff when a new team registers
      state.quiz.teams.push(team);
    },
    setOwnTeamName(state, newName) {
      if (state.quiz.team !== undefined) {
        state.quiz.team.teamName = newName;
      }
    },
    setOtherTeam(state, team: TeamInfo) {
      console.log("setOtherTeam: " + team); // eslint-disable-line no-console
      var teamInStore = state.quiz.teams.find(
        item => item.teamId === team.teamId
      );
      if (teamInStore !== undefined) {
        teamInStore.teamName = team.teamName; // throws error in console...
      }
    },
    saveSignalRConnection(state, signalrconnection) {
      state.signalrconnection = signalrconnection;
    }
  },
  actions: {
    // actions are async store updates and use the commit method to delegate
    // the action to the mutation as actions are not allowed to change the state directly.
    initTeam({ commit }, team: TeamInfo) {
      commit("setTeam", team);
      gamehub.init();
    },
    renameOtherTeam({ commit }, team: TeamInfo) {
      console.log("renameOtherTeam: " + team); // eslint-disable-line no-console
      commit("setOtherTeam", team);
    },
    processTeamRegistered({ commit, state }, teamRegistered: TeamInfo) {
      if (state.quiz.team === undefined) {
        return;
      }
      if (teamRegistered.teamId !== state.quiz.team.teamId) {
        // because the hub is not (yet) capable of notifying other teams
        // we receive our own teamRegistered notification as well.
        commit("addTeam", teamRegistered);
      }
    }
  }
};

export default new Vuex.Store<RootState>(store);
