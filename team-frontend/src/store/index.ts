import Vue from 'vue';
import Vuex, { StoreOptions } from 'vuex';
import { HubConnection } from '@aspnet/signalr';
import gamehub from '../services/gamehub';
import { Quiz, TeamInfo, UserInfo, GameStateChanged, GameState } from '../models/models';

Vue.use(Vuex);

interface RootState {
  isLoggedIn: boolean;
  team?: TeamInfo;
  otherTeams: TeamInfo[];
  gameState: GameState;
  quiz?: Quiz;
  signalrconnection?: HubConnection;
}

const store: StoreOptions<RootState> = {
  state: {
    isLoggedIn: false,
    team: undefined,
    otherTeams: [],
    gameState: GameState.Closed,
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
      state.gameState = team.gameState;
    },
    addTeam(state, team: TeamInfo) {
      // called by the signalr stuff when a new team registers
      console.log(`addTeam: ${team.teamName}`);
      team.isLoggedIn = true;
      const teamInStore = state.otherTeams.find(i => i.teamId === team.teamId);
      if (teamInStore !== undefined) {
        teamInStore.isLoggedIn = true;
        teamInStore.teamName = team.teamName;
        teamInStore.memberNames = team.memberNames;
      } else {
        state.otherTeams.push(team);
      }
    },
    setTeamLoggedOut(state, team: TeamInfo) {
      console.log(`setOtherTeamLoggedOut: ${team.teamName}`);
      const teamInStore = state.otherTeams.find(i => i.teamId === team.teamId);
      if (teamInStore !== undefined) {
        teamInStore.isLoggedIn = false;
      }
    },
    setOtherTeams(state, otherTeams: TeamInfo[]) {
      state.otherTeams = otherTeams;
    },
    setOwnTeamName(state, newName) {
      if (state.team !== undefined) {
        state.team.teamName = newName;
      }
    },
    setOwnTeamMembers(state, newMemberNames) {
      if (state.team !== undefined) {
        state.team.memberNames = newMemberNames;
      }
    },
    setOtherTeamName(state, team: TeamInfo) {
      console.log(`setOtherTeamName: ${team.teamName}`);
      const teamInStore = state.otherTeams.find(
        item => item.teamId === team.teamId
      );
      if (teamInStore !== undefined) {
        teamInStore.teamName = team.teamName;
      }
    },
    setOtherTeamMembers(state, team: TeamInfo) {
      console.log(`setOtherTeam: ${team.memberNames}`);
      const teamInStore = state.otherTeams.find(
        item => item.teamId === team.teamId
      );
      if (teamInStore !== undefined) {
        teamInStore.memberNames = team.memberNames;
      }
    },
    setGameState(state, newGameState: GameState) {
      console.log(`set game state: ${newGameState}`);
      state.gameState = newGameState;
    },
    logout(state) {
      state.team = undefined;
      state.isLoggedIn = false;
      state.quiz = undefined;
      state.otherTeams = [];
    },
    saveSignalRConnection(state, signalrconnection) {
      state.signalrconnection = signalrconnection;
    },
    clearSignalRConnection(state) {
      state.signalrconnection = undefined;
    }
  },
  actions: {
    // actions are async store updates and use the commit method to delegate
    // the action to the mutation as actions are not allowed to change the state directly.
    async initTeam({ commit }, team: TeamInfo) {
      commit('setTeam', team);
      await gamehub.init();
    },
    async logout({ commit }) {
      commit('logout');
      await gamehub.close();
    },
    processTeamNameUpdated({ commit }, team: TeamInfo) {
      console.log(`processTeamNameUpdated: ${team.teamName}`);
      commit('setOtherTeamName', team);
    },
    processTeamMembersChanged({ commit }, team: TeamInfo) {
      console.log(`processTeamMembersChanged: ${team.memberNames}`);
      commit('setOtherTeamMembers', team);
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
    },
    processTeamLoggedOut({ commit, state }, teamLoggedOut: TeamInfo) {
      if (state.team === undefined) {
        return;
      }
      if (teamLoggedOut.teamId !== state.team.teamId) {
        commit('setTeamLoggedOut', teamLoggedOut);
      }
    },
    processUserLoggedOut({ commit, state }, userLoggedOut: UserInfo) {
      // todo notify teams that the quizmaster left?
    },
    processGameStateChanged({ commit, state }, gameStateChanged: GameStateChanged) {
      if (state.gameState === undefined) {
        return;
      }
      if (state.gameState !== gameStateChanged.oldGameState) {
        console.log(`Old game state ${state.gameState} doesn't match old game state in the gameStateChanged message (${gameStateChanged.oldGameState})`);
        return;
      }
      commit('setGameState', gameStateChanged.newGameState);
    }
  }
};

export default new Vuex.Store<RootState>(store);
