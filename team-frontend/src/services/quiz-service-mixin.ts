import { AxiosResponse, AxiosError } from 'axios';
import { GameState, QuizItem } from '../models/models';
import Component, { mixins } from 'vue-class-component';
import HelperMixin from './helper-mixin';
import { ApiResponse, ImportZippedExcelQuizResponse, NavigateItemResponse } from '../models/apiResponses';
import { TeamLobbyViewModel, QmLobbyViewModel, QmInGameViewModel, TeamInGameViewModel } from '../models/viewModels';
/* eslint camelcase: "off" */
@Component
export default class QuizServiceMixin extends mixins(HelperMixin) {
  public $_quizService_addGameForQuiz(actorId: string, quizId: string, gameTitle: string, inviteCode: string): Promise<void | AxiosResponse<any>> {
    return this.$axios.post<ApiResponse>('api/game', {
      actorId, quizId, gameTitle, inviteCode
    }).catch(
      (error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      }
    );
  }

  public $_quizService_uploadQuiz(actorId: string, file: File): Promise<void | AxiosResponse<any>> {
    const formData = new FormData();
    formData.append('formFile', file);
    return this.$axios.post('api/quiz/uploadzippedexcelquiz', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
      .then((response: AxiosResponse<ImportZippedExcelQuizResponse>) => {
        this.$store.commit('addQuizViewModels', response.data.quizViewModels);
        this.$_helper_toastInfo(response);
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      });
  }
}
