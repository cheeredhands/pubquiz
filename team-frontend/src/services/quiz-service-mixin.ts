import { AxiosResponse, AxiosError } from 'axios';
import { GameState, QuizItem } from '../models/models';
import Component, { mixins } from 'vue-class-component';
import HelperMixin from './helper-mixin';
import { ApiResponse, ImportZippedExcelQuizResponse, NavigateItemResponse } from '../models/apiResponses';
import { TeamLobbyViewModel, QmLobbyViewModel, QmInGameViewModel, TeamInGameViewModel } from '../models/viewModels';
/* eslint camelcase: "off" */
@Component
export default class QuizServiceMixin extends mixins(HelperMixin) {
  public $_quizService_uploadQuiz(actorId: string, file: File) : Promise<void | AxiosResponse<any>> {
    return this.$axios.post('api/quiz/uploadzippedexcelquiz', { formFile: file })
      .then((response: AxiosResponse<ImportZippedExcelQuizResponse>) => {
        this.$store.commit('addQuizRefs', response.data.quizRefs);
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      });
  }

  public $_gameService_setGameState(actorId: string, gameId: string, newGameState: GameState):Promise<void | AxiosResponse<any>> {
    return this.$axios.post('api/game/setgamestate', {
      actorId, gameId, newGameState
    }).catch((error: AxiosError<ApiResponse>) => {
      this.$_helper_toastError(error);
    });
  }

  public $_gameService_getTeamLobby(): Promise<void> {
    return this.$axios.get('/api/game/teamlobby', { withCredentials: true })
      .then((response: AxiosResponse<TeamLobbyViewModel>) => {
        this.$store.commit('setGame', response.data.game);
        this.$store.commit('setTeams', response.data.otherTeamsInGame);
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      });
  }

  public async $_gameService_getQmLobby(): Promise<void> {
    return this.$axios
      .get('/api/game/quizmasterlobby')
      .then((response: AxiosResponse<QmLobbyViewModel>) => {
        this.$store.commit('setQmTeams', response.data.teamsInGame);
        this.$store.commit('setGame', response.data.game);
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      });
  }

  public async $_gameService_getQmInGame(): Promise<void> {
    return this.$axios
      .get('/api/game/quizmasteringame')
      .then((response: AxiosResponse<QmInGameViewModel>) => {
        this.$store.commit('setGame', response.data.game);
        this.$store.commit('setQuizItem', response.data.currentQuizItem);
        this.$store.commit('setQmTeams', response.data.teams);
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      });
  }

  public async $_gameService_getTeamInGame(): Promise<void> {
    return this.$axios
      .get('/api/game/teamingame')
      .then((response: AxiosResponse<TeamInGameViewModel>) => {
        this.$store.commit('setGame', response.data.game);
        this.$store.commit('setQuizItemViewModel', response.data.quizItemViewModel);
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      });
  }

  public async $_gameService_navigateItem(gameId: string, offset: number): Promise<void> {
    await this.$axios.post<NavigateItemResponse>('api/game/navigate', {
      gameId, offset
    }).catch(
      (error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      }
    );
  }

  public async $_gameService_getQuizItem(gameId: string, quizItemId: string): Promise<void> {
    if (this.$store.state.quizItems[quizItemId]) {
      this.$store.commit('setQuizItemFromCache', quizItemId);
    }

    await this.$axios.get<QuizItem>(`api/game/${gameId}/getquizitem/${quizItemId}`).then(response => {
      this.$store.commit('setQuizItem', response.data);
    }).catch(
      (error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      }
    );
  }

  public async $_gameService_getQuizItemViewModel(gameId: string, quizItemId: string): Promise<void> {
    if (this.$store.state.quizItemViewModels[quizItemId]) {
      this.$store.commit('setQuizItemViewModelFromCache', quizItemId);
    }

    await this.$axios.get<QuizItem>(`api/game/${gameId}/getteamquizitem/${quizItemId}`).then(response => {
      this.$store.commit('setQuizItemViewModel', response.data);
    }).catch(
      (error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      }
    );
  }

  public async $_gameService_submitInteractionResponse(quizItemId: string, interactionId: number, choiceOptionIds?: number[], response?: string): Promise<void> {
    await this.$axios.post<ApiResponse>('api/game/submitresponse', {
      quizItemId,
      interactionId,
      choiceOptionIds,
      response
    }).then(() => {
      // this.$store.commit('setQuizItemViewModel', response.data);
    }).catch(
      (error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      }
    );
  }

  public async $_gameService_correctInteraction(teamId: string, quizItemId: string, interactionId: string, correct: boolean): Promise<void> {
    await this.$axios.post<ApiResponse>('api/game/correctinteraction', { teamId, quizItemId, interactionId, correct }).then(() => {
      // nothing
    }).catch(
      (error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      }
    );
  }
}
