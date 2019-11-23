import Vue from 'vue';
import Vuex, { StoreOptions } from 'vuex';
import { HubConnection } from '@microsoft/signalr';
import gamehub from '../services/gamehub';
import { GameInfo, TeamInfo, UserInfo, GameStateChanged, GameState } from '../models/models';

Vue.use(Vuex);

interface RootState {
  isLoggedIn: boolean;
  team?: TeamInfo;
  teams: TeamInfo[];
  game?: GameInfo;
  signalrconnection?: HubConnection;

  user?: UserInfo;
  currentGameId?: string;
  gameIds: string[];
  navbarText: string;
}

const store: StoreOptions<RootState> = {
  state: {
    isLoggedIn: false,
    team: undefined,
    teams: [],
    game: undefined,
    signalrconnection: undefined,

    user: undefined,
    currentGameId: undefined,
    gameIds: [],
    navbarText: 'Quizr'
  },
  getters: {},
  mutations: {
    // mutations are sync store updates
    setUser(state, user: UserInfo) {
      // called when a quizmaster logs in succesfully
      state.user = user;
      state.isLoggedIn = true;
      state.currentGameId = user.currentGameId;
      state.gameIds = user.gameIds;
    },
    setTeam(state, team: TeamInfo) {
      // called when the current team registers succesfully
      state.team = team;
      state.currentGameId = team.currentGameId;
      state.isLoggedIn = true;
    },
    addTeam(state, team: TeamInfo) {
      // called by the signalr stuff when a new team registers
      console.log(`addTeam: ${team.teamName}`);
      team.isLoggedIn = true;
      const teamInStore = state.teams.find(i => i.teamId === team.teamId);
      if (teamInStore !== undefined) {
        teamInStore.isLoggedIn = true;
        teamInStore.teamName = team.teamName;
        teamInStore.memberNames = team.memberNames;
      } else {
        state.teams.push(team);
      }
    },
    removeTeam(state, team: TeamInfo) {
      console.log(`removeTeam: ${team.teamId}`);
      const teamInStore = state.teams.find(i => i.teamId === team.teamId);
      if (teamInStore !== undefined) {
        state.teams = state.teams.filter(t => t.teamId !== team.teamId);
      }
    },
    setTeamLoggedOut(state, team: TeamInfo) {
      console.log(`setOtherTeamLoggedOut: ${team.teamName}`);
      const teamInStore = state.teams.find(i => i.teamId === team.teamId);
      if (teamInStore !== undefined) {
        teamInStore.isLoggedIn = false;
      }
    },
    setTeams(state, teams: TeamInfo[]) {
      state.teams = teams;
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
      const teamInStore = state.teams.find(
        item => item.teamId === team.teamId
      );
      if (teamInStore !== undefined) {
        teamInStore.teamName = team.teamName;
      }
    },
    setOtherTeamMembers(state, team: TeamInfo) {
      console.log(`setOtherTeam: ${team.memberNames}`);
      const teamInStore = state.teams.find(
        item => item.teamId === team.teamId
      );
      if (teamInStore !== undefined) {
        teamInStore.memberNames = team.memberNames;
      }
    },
    setGameState(state, newGameState: GameState) {
      console.log(`set game state: ${newGameState}`);
      if (state.game === undefined) {
        return;
      }
      state.game.state = newGameState;
    },
    setGame(state, currentGame: GameInfo) {
      state.game = currentGame;
    },
    logout(state) {
      state.team = undefined;
      state.isLoggedIn = false;
      state.game = undefined;
      state.currentGameId = undefined;
      state.teams = [];
    },
    saveSignalRConnection(state, signalrconnection) {
      state.signalrconnection = signalrconnection;
    },
    clearSignalRConnection(state) {
      state.signalrconnection = undefined;
    },
    setNavbarText(state, text: string){
      state.navbarText = text;
    }
  },
  actions: {
    // actions are async store updates and use the commit method to delegate
    // the action to the mutation as actions are not allowed to change the state directly.
    async storeToken({ commit }, jwt: string) {
      localStorage.setItem('token', jwt);
    },
    async initTeam({ commit }, team: TeamInfo) {
      commit('setTeam', team);
      await gamehub.init();
    },
    async initQuizMaster({ commit }, user: UserInfo) {
      commit('setUser', user);
      await gamehub.init();
    },
    async logout({ commit }) {
      commit('logout');
      await gamehub.close();
      localStorage.removeItem('token');
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
      console.log(`processTeamRegistered: ${teamRegistered.teamName}`);
      if (state.user !== undefined) {
        commit('addTeam', teamRegistered);
      } else if (state.team !== undefined && teamRegistered.teamId !== state.team.teamId) {
        commit('addTeam', teamRegistered);
      }
    },
    processTeamLoggedOut({ commit, state }, teamLoggedOut: TeamInfo) {
      console.log(`processTeamLoggedOut: ${teamLoggedOut.teamName}`);
      if (state.user !== undefined) {
        commit('setTeamLoggedOut', teamLoggedOut);
      } else if (state.team !== undefined && teamLoggedOut.teamId !== state.team.teamId) {
        commit('setTeamLoggedOut', teamLoggedOut);
      }
    },
    processUserLoggedOut({ commit, state }, userLoggedOut: UserInfo) {
      // todo notify teams that the quizmaster left?
    },
    processGameStateChanged({ commit, state }, gameStateChanged: GameStateChanged) {
      if (state.game === undefined) {
        return;
      }
      if (state.game.state !== gameStateChanged.oldGameState) {
        console.log(`Old game state ${state.game.state} doesn't match old game state in the gameStateChanged message (${gameStateChanged.oldGameState})`);
        return;
      }
      commit('setGameState', gameStateChanged.newGameState);
    },
    processTeamDeleted({ commit, state }, deletedTeam: TeamInfo) {
      if (state.team !== undefined && deletedTeam.teamId === state.team.teamId) {
        commit('logout');
      } else {
        commit('removeTeam', deletedTeam);
      }
    }
  }
};

export default new Vuex.Store<RootState>(store);
