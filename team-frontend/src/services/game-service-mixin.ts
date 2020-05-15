import { AxiosResponse, AxiosError } from 'axios';
import { ApiResponse, GameState,  QuizItem, NavigateItemResponse } from '../models/models';
import Component, { mixins } from 'vue-class-component';
import HelperMixin from './helper-mixin';

@Component
export default class GameServiceMixin extends mixins(HelperMixin) {

    public $_gameService_setGameState(actorId: string, gameId: string, newGameState: GameState): Promise<AxiosResponse> {
        return this.$axios.post('api/game/setgamestate', {
            actorId, gameId, newGameState
        });
    }

    public async $_gameService_navigateItem(gameId: string, offset: number) {
        await this.$axios.post<NavigateItemResponse>('api/game/navigate', {
            gameId, offset
        }).then(async response => {
            await this.$_gameService_getQuizItem(gameId, response.data.quizItemId);
        }).catch(
            (error: AxiosError<ApiResponse>) => {
                this.$_helper_toastError(error);
            }
        );
    }

    public async $_gameService_getQuizItem(gameId: string, quizItemId: string) {
        if (this.$store.state.quizItems.has(quizItemId)) {
            this.$store.commit('setQuizItemFromCache', quizItemId);
            return;
        }

        await this.$axios.get<QuizItem>(`api/game/${gameId}/getquizitem/${quizItemId}`).then(response => {
            this.$store.commit('setQuizItem', response.data);
        }).catch(
            (error: AxiosError<ApiResponse>) => {
                this.$_helper_toastError(error);
            }
        );
    }
}