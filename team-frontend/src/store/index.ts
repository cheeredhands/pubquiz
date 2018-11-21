import Vue from 'vue';
import Vuex, { StoreOptions } from 'vuex';
import { HubConnection } from '@aspnet/signalr';
import gamehub from '../services/gamehub';
import { Quiz, TeamInfo } from '../models/models';

Vue.use(Vuex);

interface RootState {
  isLoggedIn: boolean;
  team?: TeamInfo;
  otherTeams: TeamInfo[];
  quiz?: Quiz;
  signalrconnection?: HubConnection;
}

const store: StoreOptions<RootState> = {
  state: {
    isLoggedIn: false,
    team: undefined,
    otherTeams: [],
    quiz: undefined,
    signalrconnection: undefined
  },
  getters: {},
  mutations: {
    // mutations are sync store updates
    setTeam(state, team: TeamInfo) {
      // called when the current team registers succesfully
      state.team = team;
      state.isLoggedIn = true;
    },
    addTeam(state, team: TeamInfo) {
      // called by the signalr stuff when a new team registers
      state.otherTeams.push(team);
    },
    setOtherTeams(state, otherTeams: TeamInfo[]) {
      state.otherTeams = otherTeams;
    },
    setOwnTeamName(state, newName) {
      if (state.team !== undefined) {
        state.team.teamName = newName;
      }
    },
    setOtherTeamName(state, team: TeamInfo) {
      console.log(`setOtherTeam: ${team}`); // tslint:disable-line no-console
      const teamInStore = state.otherTeams.find(
        item => item.teamId === team.teamId
      );
      if (teamInStore !== undefined) {
        teamInStore.teamName = team.teamName;
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
      commit('setTeam', team);
      gamehub.init();
    },
    renameOtherTeam({ commit }, team: TeamInfo) {
      console.log(`renameOtherTeam: ${team}`); // tslint:disable-line no-console
      commit('setOtherTeamName', team);
    },
    processTeamRegistered({ commit, state }, teamRegistered: TeamInfo) {
      if (state.team === undefined) {
        return;
      }
      if (teamRegistered.teamId !== state.team.teamId) {
        // because the hub is not (yet) capable of notifying other teams
        // we receive our own teamRegistered notification as well.
        commit('addTeam', teamRegistered);
      }
    }
  }
};

export default new Vuex.Store<RootState>(store);
