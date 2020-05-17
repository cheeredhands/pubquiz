import Vue from 'vue';
import Vuex, { StoreOptions } from 'vuex';
import { HubConnection } from '@microsoft/signalr';
import gamehub from '../services/gamehub';
import { User, GameState, ItemNavigatedMessage, QuizItem, Team, Game } from '../models/models';
import { TeamLoggedOutMessage, TeamRegisteredMessage, TeamNameUpdatedMessage, TeamMembersChangedMessage, TeamDeletedMessage, GameStateChangedMessage } from '@/models/messages';

Vue.use(Vuex);

interface RootState {
  isLoggedIn: boolean;
  team?: Team;
  teams: Team[];
  game?: Game;
  quizItem?: QuizItem;
  quizItems: Map<string, QuizItem>;
  signalrconnection?: HubConnection;

  user?: User;
  currentGameId?: string;
  gameIds: string[];
}

const storeOpts: StoreOptions<RootState> = {
  state: {
    isLoggedIn: false,
    team: undefined,
    teams: [],
    game: undefined,
    quizItem: undefined,
    quizItems: new Map<string, QuizItem>(),
    signalrconnection: undefined,

    user: undefined,
    currentGameId: undefined,
    gameIds: []
  },
  getters: {
    game: state => state.game || {},
    userId: state => state.user?.userId || '',
    quizItem: state => state.quizItem || {},
    teamId: state => state.team?.teamId || '',
    teamName: state => state.team?.teamName || '',
    memberNames: state => state.team?.memberNames || ''
  },
  mutations: {
    // mutations are sync store updates
    setUser(state, user: User) {
      // called when a quizmaster logs in succesfully
      state.user = user;
      state.isLoggedIn = true;
      state.currentGameId = user.currentGameId;
      state.gameIds = user.gameIds;
    },
    setTeam(state, team: Team) {
      // called when the current team registers succesfully
      state.team = team;
      state.currentGameId = team.currentGameId;
      state.isLoggedIn = true;
    },
    addTeam(state, team: Team) {
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
    removeTeam(state, team: Team) {
      console.log(`removeTeam: ${team.teamId}`);
      const teamInStore = state.teams.find(i => i.teamId === team.teamId);
      if (teamInStore !== undefined) {
        state.teams = state.teams.filter(t => t.teamId !== team.teamId);
      }
    },
    setTeamLoggedOut(state, team: TeamLoggedOutMessage) {
      console.log(`setOtherTeamLoggedOut: ${team.teamName}`);
      const teamInStore = state.teams.find(i => i.teamId === team.teamId);
      if (teamInStore !== undefined) {
        teamInStore.isLoggedIn = false;
      }
    },
    setTeams(state, teams: Team[]) {
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
    setOtherTeamName(state, team: Team) {
      console.log(`setOtherTeamName: ${team.teamName}`);
      const teamInStore = state.teams.find(
        item => item.teamId === team.teamId
      );
      if (teamInStore !== undefined) {
        teamInStore.teamName = team.teamName;
      }
    },
    setOtherTeamMembers(state, team: Team) {
      console.log(`setOtherTeamMembers: ${team.memberNames}`);
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
    setGame(state, currentGame: Game) {
      state.game = currentGame;
    },
    setCurrentItem(state, itemNavigationInfo: ItemNavigatedMessage) {
      if (state.game === undefined) {
        return;
      }
      state.game.currentSectionQuizItemCount = itemNavigationInfo.newSectionQuizItemCount;
      state.game.currentSectionIndex = itemNavigationInfo.newSectionIndex;
      state.game.currentSectionId = itemNavigationInfo.newSectionId;
      state.game.currentQuizItemId = itemNavigationInfo.newQuizItemId;
      state.game.currentQuizItemIndexInSection = itemNavigationInfo.newQuizItemIndexInSection;
      state.game.currentQuizItemIndexInTotal = itemNavigationInfo.newQuizItemIndexInTotal;
      state.game.currentQuestionIndexInTotal = itemNavigationInfo.newQuestionIndexInTotal;
    },
    setQuizItem(state, quizItem: QuizItem) {
      state.quizItem = quizItem;
      if (!state.quizItems.has(quizItem.id)) {
        state.quizItems.set(quizItem.id, quizItem);
      }
    },
    setQuizItemFromCache(state, quizItemId: string) {
      const quizItem = state.quizItems.get(quizItemId);
      state.quizItem = quizItem;
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
    }
  },
  actions: {
    // actions are async store updates and use the commit method to delegate
    // the action to the mutation as actions are not allowed to change the state directly.
    async storeToken({ commit }, jwt: string) {
      localStorage.setItem('token', jwt);
    },
    async initTeam({ commit }, team: TeamDeletedMessage) {
      commit('setTeam', team);
      await gamehub.init();
    },
    async initQuizMaster({ commit }, user: User) {
      commit('setUser', user);
      await gamehub.init();
    },
    async logout({ commit }) {
      commit('logout');
      await gamehub.close();
      localStorage.removeItem('token');
    },
    processTeamNameUpdated({ commit }, team: TeamNameUpdatedMessage) {
      console.log(`processTeamNameUpdated: ${team.teamName}`);
      commit('setOtherTeamName', team);
    },
    processTeamMembersChanged({ commit }, team: TeamMembersChangedMessage) {
      console.log(`processTeamMembersChanged: ${team.memberNames}`);
      commit('setOtherTeamMembers', team);
    },
    processTeamRegistered({ commit, state }, team: TeamRegisteredMessage) {
      console.log(`processTeamRegistered: ${team.teamName}`);
      if (state.user !== undefined) {
        commit('addTeam', team);
      } else if (state.team !== undefined && team.teamId !== state.team.teamId) {
        commit('addTeam', team);
      }
    },
    processTeamLoggedOut({ commit, state }, team: TeamLoggedOutMessage) {
      console.log(`processTeamLoggedOut: ${team.teamName}`);
      if (state.user !== undefined) {
        commit('setTeamLoggedOut', team);
      } else if (state.team !== undefined && team.teamId !== state.team.teamId) {
        commit('setTeamLoggedOut', team);
      }
    },
    processUserLoggedOut({ commit, state }, userLoggedOut: User) {
      // todo notify teams that the quizmaster left?
    },
    processGameStateChanged({ commit, state }, gameStateChangedMessage: GameStateChangedMessage) {
      if (state.game === undefined) {
        return;
      }
      if (state.game.state !== gameStateChangedMessage.oldGameState) {
        console.log(`Old game state ${state.game.state} doesn't match old game state in the gameStateChanged message (${gameStateChangedMessage.oldGameState})`);
        return;
      }
      commit('setGameState', gameStateChangedMessage.newGameState);
    },
    processTeamDeleted({ commit, state }, team: TeamDeletedMessage) {
      if (state.team !== undefined && team.teamId === state.team.teamId) {
        commit('logout');
      } else {
        commit('removeTeam', team);
      }
    },
    processItemNavigated({ commit, state }, itemNavigatedMessage: ItemNavigatedMessage) {
      if (state.game === undefined) {
        return;
      }
      commit('setCurrentItem', itemNavigatedMessage);
    }
  }
};

export default new Vuex.Store<RootState>(storeOpts);
