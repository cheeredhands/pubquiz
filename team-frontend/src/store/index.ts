import Vue from 'vue';
import Vuex, { StoreOptions } from 'vuex';
import { HubConnection } from '@microsoft/signalr';
import gamehub from '../services/gamehub';
import { User, QuizItem, Team, Game } from '../models/models';
import { TeamLoggedOutMessage, ItemNavigatedMessage, TeamRegisteredMessage, TeamNameUpdatedMessage, TeamMembersChangedMessage, TeamDeletedMessage, GameStateChangedMessage, AnswerScoredMessage, QmTeamRegisteredMessage, TeamConnectionChangedMessage, GameSelectedMessage, GameCreatedMessage } from '../models/messages';
import { GameViewModel, QuizItemViewModel, QuizViewModel, TeamViewModel } from '../models/viewModels';
import { WhoAmIResponse } from '@/models/apiResponses';
/* eslint no-console: "off" */

Vue.use(Vuex);
export interface RootState {
  isLoggedIn: boolean;
  team?: Team;
  teams: TeamViewModel[];
  game?: Game;
  quizItem?: QuizItem;
  quizItems: Record<string, QuizItem>; //  { [id: string]: QuizItem; };
  quizItemViewModel?: QuizItemViewModel;
  quizItemViewModels: Record<string, QuizItemViewModel>;
  signalrconnection?: HubConnection;

  user?: User;
  currentGameId?: string;
  quizViewModels: QuizViewModel[];
  gameViewModels: GameViewModel[];
  qmTeams: Team[];
  debounceMs: number;
}

const storeOpts: StoreOptions<RootState> = {
  state: {
    isLoggedIn: false,
    team: undefined,
    teams: [],
    game: undefined,
    quizItem: undefined,
    quizItems: {},
    quizItemViewModel: undefined,
    quizItemViewModels: {},
    signalrconnection: undefined,

    user: undefined,
    currentGameId: undefined,
    quizViewModels: [],
    gameViewModels: [],
    qmTeams: [],
    debounceMs: parseInt(process.env.VUE_APP_DEBOUNCE_MS || '1500', 10)
  },
  getters: {
    user: state => state.user || {},
    team: state => state.team || {},
    game: state => state.game || {},
    userId: state => state.user?.userId || '',
    userName: state => state.user?.userName || '',
    quizItem: state => state.quizItem || {},
    quizItemViewModel: state => state.quizItemViewModel || {},
    teamId: state => state.team?.id || '',
    teamName: state => state.team?.name || '',
    memberNames: state => state.team?.memberNames || '',
    currentQuizItemId: state => state.game?.currentQuizItemId || '',
    throttleMs: state => state.debounceMs,
    recoveryCode: state => state.team?.recoveryCode || '',
    qmTeams: state => state.qmTeams || [],
    quizViewModels: state => state.quizViewModels || [],
    gameViewModels: state => state.gameViewModels || []
  },
  mutations: {
    // mutations are sync store updates
    setUser(state, user: User) {
      state.user = user;
      state.isLoggedIn = true;
      state.currentGameId = user.currentGameId;
    },
    setTeam(state, team: Team) {
      state.team = team;
      state.currentGameId = team.gameId;
      state.isLoggedIn = true;
    },
    addTeam(state, team: TeamRegisteredMessage) {
      const teamInStore = state.teams.find(i => i.id === team.teamId);
      if (teamInStore !== undefined) {
        teamInStore.isLoggedIn = true;
        teamInStore.name = team.name;
        teamInStore.memberNames = team.memberNames;
      } else {
        state.teams.push({
          id: team.teamId,
          name: team.name,
          memberNames: team.memberNames,
          isLoggedIn: true,
          connectionCount: 1
        });
      }
    },
    addQmTeam(state, team: Team) {
      let teamInStore = state.qmTeams.find(i => i.id === team.id);
      if (teamInStore !== undefined) {
        teamInStore = team;
        // Vue.set(state.qmTeams, teamInStoreIndex, team);
      } else {
        state.qmTeams.push(team);
      }
    },
    addQuizViewModels(state, quizViewModels: QuizViewModel[]) {
      for (let i = 0; i < quizViewModels.length; i++) {
        const vm = quizViewModels[i];
        const quizInStore = state.quizViewModels.find(v => v.id === vm.id);
        if (quizInStore === undefined) {
          state.quizViewModels.push(vm);
        }
      }
    },
    removeTeam(state, teamId: string) {
      const teamInStore = state.teams.find(i => i.id === teamId);
      if (teamInStore !== undefined) {
        state.teams = state.teams.filter(t => t.id !== teamId);
      }
      const qmTeamInStore = state.qmTeams.find(i => i.id === teamId);
      if (qmTeamInStore !== undefined) {
        state.qmTeams = state.qmTeams.filter(t => t.id !== teamId);
      }
    },
    setTeamLoggedOut(state, teamId: string) {
      const teamInStore = state.teams.find(i => i.id === teamId);
      if (teamInStore !== undefined) {
        teamInStore.isLoggedIn = false;
      }
      const qmTeamInStore = state.qmTeams.find(i => i.id === teamId);
      if (qmTeamInStore !== undefined) {
        qmTeamInStore.isLoggedIn = false;
      }
    },
    setTeamConnectionCount(state, message: TeamConnectionChangedMessage) {
      const teamInStore = state.teams.find(i => i.id === message.teamId);
      if (teamInStore !== undefined) {
        teamInStore.connectionCount = message.connectionCount;
        teamInStore.isLoggedIn = teamInStore.connectionCount > 0;
      }
      const qmTeamInStore = state.qmTeams.find(i => i.id === message.teamId);
      if (qmTeamInStore !== undefined) {
        qmTeamInStore.connectionCount = message.connectionCount;
        qmTeamInStore.isLoggedIn = qmTeamInStore.connectionCount > 0;
      }
    },
    setTeams(state, teams: TeamViewModel[]) {
      state.teams = teams;
    },
    setQmTeams(state, teams: Team[]) {
      state.qmTeams = teams;
    },
    setQmTeamAnswer(state, message: AnswerScoredMessage) {
      const team = state.qmTeams.find(t => t.id === message.teamId);
      if (team === undefined) { return; }
      team.totalScore = message.totalTeamScore;
      team.scorePerQuizSection = message.scorePerQuizSection;
      // Change detection caveats https://vuejs.org/v2/guide/reactivity.html#For-Arrays
      Vue.set(team.answers, message.quizItemId, message.answer);
    },
    setOwnTeamName(state, newName) {
      if (state.team !== undefined) {
        state.team.name = newName;
      }
    },
    setOwnTeamMembers(state, newMemberNames) {
      if (state.team !== undefined) {
        state.team.memberNames = newMemberNames;
      }
    },
    setOtherTeamName(state, message: TeamNameUpdatedMessage) {
      const teamInStore = state.teams.find(t => t.id === message.teamId);
      if (teamInStore !== undefined) {
        teamInStore.name = message.name;
      }
      const qmTeamInStore = state.qmTeams.find(t => t.id === message.teamId);
      if (qmTeamInStore !== undefined) {
        qmTeamInStore.name = message.name;
      }
    },
    setOtherTeamMembers(state, message: TeamMembersChangedMessage) {
      const teamInStore = state.teams.find(t => t.id === message.teamId);
      const qmTeamInStore = state.qmTeams.find(t => t.id === message.teamId);
      if (teamInStore !== undefined) {
        // use Vue.set in case memberNames is undefined (https://vuejs.org/v2/guide/reactivity.html#For-Objects)
        Vue.set(teamInStore, 'memberNames', message.memberNames);
      }
      if (qmTeamInStore !== undefined) {
        // use Vue.set in case memberNames is undefined (https://vuejs.org/v2/guide/reactivity.html#For-Objects)
        Vue.set(qmTeamInStore, 'memberNames', message.memberNames);
      }
    },
    setGameState(state, message: GameStateChangedMessage) {
      if (state.game !== undefined && state.currentGameId === message.gameId) {
        state.game.state = message.newGameState;
      }
      const gameVmInStore = state.gameViewModels.find(g => g.id === message.gameId);
      if (gameVmInStore !== undefined) {
        Vue.set(gameVmInStore, 'gameState', message.newGameState);
      }
    },
    setGame(state, currentGame: Game) {
      state.game = currentGame;
      if (currentGame === undefined) {
        state.currentGameId = undefined;
      }
    },
    setGames(state, gameViewModels: GameViewModel[]) {
      state.gameViewModels = gameViewModels;
    },
    setQuizzes(state, quizViewModels: QuizViewModel[]) {
      state.quizViewModels = quizViewModels;
    },
    addGame(state, game: GameViewModel) {
      state.gameViewModels.push(game);
    },
    setCurrentItem(state, message: ItemNavigatedMessage) {
      if (state.game === undefined) {
        return;
      }
      state.game.currentSectionQuizItemCount = message.newSectionQuizItemCount;
      state.game.currentSectionIndex = message.newSectionIndex;
      state.game.currentSectionId = message.newSectionId;
      state.game.currentSectionTitle = message.newSectionTitle;
      state.game.currentQuizItemId = message.newQuizItemId;
      state.game.currentQuizItemIndexInSection = message.newQuizItemIndexInSection;
      state.game.currentQuizItemIndexInTotal = message.newQuizItemIndexInTotal;
      state.game.currentQuestionIndexInTotal = message.newQuestionIndexInTotal;
    },
    setQuizItem(state, quizItem: QuizItem) {
      state.quizItem = quizItem;
      Vue.set(state.quizItems, quizItem.id, quizItem);
      // state.quizItems[quizItem.id] = quizItem;
    },
    setQuizItemFromCache(state, quizItemId: string) {
      const quizItem = state.quizItems[quizItemId];
      state.quizItem = quizItem;
    },
    setQuizItemViewModel(state, quizItemViewModel: QuizItemViewModel) {
      state.quizItemViewModel = quizItemViewModel;
      Vue.set(state.quizItemViewModels, quizItemViewModel.id, quizItemViewModel);
      // state.quizItemViewModels[quizItemViewModel.id] = quizItemViewModel;
    },
    setQuizItemViewModelFromCache(state, quizItemId: string) {
      const quizItemViewModel = state.quizItemViewModels[quizItemId];
      state.quizItemViewModel = quizItemViewModel;
    },
    logout(state) {
      state.team = undefined;
      state.user = undefined;
      state.isLoggedIn = false;
      state.game = undefined;
      state.currentGameId = undefined;
      state.teams = [];
      state.gameViewModels = [];
      state.quizViewModels = [];
      state.qmTeams = [];
      state.quizItem = undefined;
      state.quizItems = {};
      state.quizItemViewModel = undefined;
      state.quizItemViewModels = {};
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
    async initTeam({ commit }, team: Team) {
      commit('setTeam', team);
      await gamehub.init();
    },
    async initQuizMaster({ commit }, user: WhoAmIResponse) {
      commit('setUser', user);
      await gamehub.init();
    },
    async logout({ commit }) {
      commit('logout');
      await gamehub.close();
      localStorage.removeItem('token');
    },
    processTeamConnectionChanged({ commit, state }, message: TeamConnectionChangedMessage) {
      console.log(`processTeamConnectionChanged: ${message.name}`);
      if (state.user !== undefined) {
        commit('setTeamConnectionCount', message);
      } else if (state.team !== undefined && message.teamId !== state.team.id) {
        commit('setTeamConnectionCount', message);
      }
    },
    processTeamNameUpdated({ commit }, message: TeamNameUpdatedMessage) {
      console.log(`processTeamNameUpdated: ${message.name}`);
      commit('setOtherTeamName', message);
    },
    processTeamMembersChanged({ commit }, message: TeamMembersChangedMessage) {
      console.log(`processTeamMembersChanged: ${message.memberNames}`);
      commit('setOtherTeamMembers', message);
    },
    processTeamRegistered({ commit, state }, message: TeamRegisteredMessage) {
      console.log(`processTeamRegistered: ${message.name}`);
      if (state.user !== undefined) {
        commit('addTeam', message);
      } else if (state.team !== undefined && message.teamId !== state.team.id) {
        commit('addTeam', message);
      }
    },
    processQmTeamRegistered({ commit, state }, message: QmTeamRegisteredMessage) {
      console.log(`processQmTeamRegistered: ${message.team.name}`);
      if (state.user !== undefined) {
        commit('addQmTeam', message.team);
      }
    },
    processTeamLoggedOut({ commit, state }, message: TeamLoggedOutMessage) {
      console.log(`processTeamLoggedOut: ${message.name}`);
      commit('setTeamLoggedOut', message.teamId);
    },
    processUserLoggedOut() { // { commit, state }, userLoggedOut: User) {
      // todo notify teams that the quizmaster left?
    },
    processGameStateChanged({ commit, state }, message: GameStateChangedMessage) {
      if (state.game !== undefined && state.game.id === message.gameId) {
        if (state.game.state !== message.oldGameState) {
          console.log(`Old game state ${state.game.state} doesn't match old game state in the gameStateChanged message (${message.oldGameState})`);
          return;
        }
      }
      commit('setGameState', message);
    },
    processTeamDeleted({ commit, state }, message: TeamDeletedMessage) {
      if (state.team !== undefined && message.teamId === state.team.id) {
        commit('logout');
      } else {
        commit('removeTeam', message.teamId);
      }
    },
    processItemNavigated({ commit, state }, message: ItemNavigatedMessage) {
      if (state.game === undefined) {
        return;
      }
      commit('setCurrentItem', message);
    },
    processInteractionResponseAdded() { // { commit, state }, message: InteractionResponseAddedMessage) {
      // TODO remove?
    },
    processAnswerScored({ commit }, message: AnswerScoredMessage) {
      // const team = state.qmTeams.find(t => t.id === answerScoredMessage.teamId);
      // if (team === undefined) { return; }
      commit('setQmTeamAnswer', message);
    },
    processGameSelected({ commit }, message: GameSelectedMessage) {
      commit('setQmTeams', message.qmLobbyViewModel.teamsInGame);
      commit('setGame', message.qmLobbyViewModel.game);
    },
    processGameCreated({ commit }, message: GameCreatedMessage) {
      commit('addGame', message.qmGameViewModel);
    }
  }
};

export default new Vuex.Store<RootState>(storeOpts);
