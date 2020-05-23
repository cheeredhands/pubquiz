import Vue from 'vue';
import Vuex, { StoreOptions } from 'vuex';
import { HubConnection } from '@microsoft/signalr';
import gamehub from '../services/gamehub';
import { User, GameState, ItemNavigatedMessage, QuizItem, Team, Game } from '../models/models';
import { TeamLoggedOutMessage, TeamRegisteredMessage, TeamNameUpdatedMessage, TeamMembersChangedMessage, TeamDeletedMessage, GameStateChangedMessage, InteractionResponseAddedMessage } from '../models/messages';
import { TeamFeedViewModel, TeamRankingViewModel, QuizItemViewModel, TeamViewModel } from '../models/viewModels';

Vue.use(Vuex);

interface RootState {
  isLoggedIn: boolean;
  team?: Team;
  teams: TeamViewModel[];
  game?: Game;
  quizItem?: QuizItem;
  quizItems: Map<string, QuizItem>;
  quizItemViewModel?: QuizItemViewModel;
  quizItemViewModels: Map<string, QuizItemViewModel>;
  signalrconnection?: HubConnection;

  user?: User;
  currentGameId?: string;
  gameIds: string[];
  teamFeed?: TeamFeedViewModel;
  teamRanking?: TeamRankingViewModel;

  debounceMs: number;
}

const storeOpts: StoreOptions<RootState> = {
  state: {
    isLoggedIn: false,
    team: undefined,
    teams: [],
    game: undefined,
    quizItem: undefined,
    quizItems: new Map<string, QuizItem>(),
    quizItemViewModel: undefined,
    quizItemViewModels: new Map<string, QuizItemViewModel>(),
    signalrconnection: undefined,

    user: undefined,
    currentGameId: undefined,
    gameIds: [],
    teamFeed: undefined,
    teamRanking: undefined,
    debounceMs: parseInt(process.env.VUE_APP_DEBOUNCE_MS || '500', 10)
  },
  getters: {
    game: state => state.game || {},
    userId: state => state.user?.userId || '',
    quizItem: state => state.quizItem || {},
    quizItemViewModel: state => state.quizItemViewModel || {},
    teamId: state => state.team?.id || '',
    teamName: state => state.team?.name || '',
    memberNames: state => state.team?.memberNames || '',
    teamFeed: state => state.teamFeed || {},
    teamRanking: state => state.teamRanking || {},
    currentQuizItemId: state => state.game?.currentQuizItemId || '',
    debounceMs: state => state.debounceMs,
    recoveryCode: state =>state.team?.recoveryCode || ''
  },
  mutations: {
    // mutations are sync store updates
    setUser(state, user: User) {
      state.user = user;
      state.isLoggedIn = true;
      state.currentGameId = user.currentGameId;
      state.gameIds = user.gameIds;
    },
    setTeam(state, team: Team) {
      state.team = team;
      state.currentGameId = team.gameId;
      state.isLoggedIn = true;
    },
    setTeamFeed(state, teamFeed: TeamFeedViewModel) {
      state.teamFeed = teamFeed;
    },
    setTeamRanking(state, teamRanking: TeamRankingViewModel) {
      state.teamRanking = teamRanking;
    },
    addTeam(state, team: TeamRegisteredMessage) {
      console.log(`addTeam: ${team.name}`);
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
          isLoggedIn: true
        });
      }
    },
    removeTeam(state, team: TeamViewModel) {
      console.log(`removeTeam: ${team.id}`);
      const teamInStore = state.teams.find(i => i.id === team.id);
      if (teamInStore !== undefined) {
        state.teams = state.teams.filter(t => t.id !== team.id);
      }
    },
    setTeamLoggedOut(state, team: TeamLoggedOutMessage) {
      console.log(`setOtherTeamLoggedOut: ${team.name}`);
      const teamInStore = state.teams.find(i => i.id === team.teamId);
      if (teamInStore !== undefined) {
        teamInStore.isLoggedIn = false;
      }
    },
    setTeams(state, teams: TeamViewModel[]) {
      state.teams = teams;
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
    setOtherTeamName(state, team: TeamViewModel) {
      console.log(`setOtherTeamName: ${team.name}`);
      const teamInStore = state.teams.find(
        item => item.id === team.id
      );
      if (teamInStore !== undefined) {
        teamInStore.name = team.name;
      }
    },
    setOtherTeamMembers(state, team: TeamViewModel) {
      console.log(`setOtherTeamMembers: ${team.memberNames}`);
      const teamInStore = state.teams.find(
        item => item.id === team.id
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
    setQuizItemViewModel(state, quizItemViewModel: QuizItemViewModel) {
      state.quizItemViewModel = quizItemViewModel;
      if (!state.quizItemViewModels.has(quizItemViewModel.id)) {
        state.quizItemViewModels.set(quizItemViewModel.id, quizItemViewModel);
      }
    },
    setQuizItemViewModelFromCache(state, quizItemId: string) {
      const quizItemViewModel = state.quizItemViewModels.get(quizItemId);
      state.quizItemViewModel = quizItemViewModel;
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
    async initTeam({ commit }, team: Team) {
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
      console.log(`processTeamNameUpdated: ${team.name}`);
      commit('setOtherTeamName', team);
    },
    processTeamMembersChanged({ commit }, team: TeamMembersChangedMessage) {
      console.log(`processTeamMembersChanged: ${team.memberNames}`);
      commit('setOtherTeamMembers', team);
    },
    processTeamRegistered({ commit, state }, team: TeamRegisteredMessage) {
      console.log(`processTeamRegistered: ${team.name}`);
      if (state.user !== undefined) {
        commit('addTeam', team);
      } else if (state.team !== undefined && team.teamId !== state.team.id) {
        commit('addTeam', team);
      }
    },
    processTeamLoggedOut({ commit, state }, team: TeamLoggedOutMessage) {
      console.log(`processTeamLoggedOut: ${team.name}`);
      if (state.user !== undefined) {
        commit('setTeamLoggedOut', team);
      } else if (state.team !== undefined && team.teamId !== state.team.id) {
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
      if (state.team !== undefined && team.teamId === state.team.id) {
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
    },
    processInteractionResponseAdded({ commit, state }, interactionResponseAddedMessage: InteractionResponseAddedMessage) {
      // TODO update teamfeed
    }
  }
};

export default new Vuex.Store<RootState>(storeOpts);
