import { AxiosResponse, AxiosError } from 'axios';
import { GameState, QuizItem } from '../models/models';
import Component, { mixins } from 'vue-class-component';
import HelperMixin from './helper-mixin';
import { ApiResponse, NavigateItemResponse } from '../models/apiResponses';
import { TeamLobbyViewModel, QmLobbyViewModel, QmInGameViewModel } from '../models/viewModels';

@Component
export default class GameServiceMixin extends mixins(HelperMixin) {

    public $_gameService_setGameState(actorId: string, gameId: string, newGameState: GameState) {
        return this.$axios.post('api/game/setgamestate', {
            actorId, gameId, newGameState
        }).then((response: AxiosResponse<ApiResponse>) => {
            // this.$router.push({ name: 'QuizMasterGame' });
        })
            .catch((error: AxiosError<ApiResponse>) => {
                this.$_helper_toastError(error);
            });;
    }

    public $_gameService_getTeamLobby() {
        return this.$axios.get('/api/game/teamlobby', { withCredentials: true })
            .then((response: AxiosResponse<TeamLobbyViewModel>) => {
                // this.$store.commit('setTeam', response.data.team);
                this.$store.commit('setTeams', response.data.otherTeamsInGame);
            })
            .catch((error: AxiosError<ApiResponse>) => {
                this.$_helper_toastError(error);
            });
    }

    public async $_gameService_getQmLobby() {
        // get team lobby view model
        this.$axios
            .get('/api/game/quizmasterlobby')
            .then((response: AxiosResponse<QmLobbyViewModel>) => {
                this.$store.commit('setTeams', response.data.teamsInGame);
                this.$store.commit('setGame', response.data.game);
            })
            .catch((error: AxiosError<ApiResponse>) => {
                this.$_helper_toastError(error);
            });
    }

    public async $_gameService_getQmInGame() {
        // get team lobby view model
        this.$axios
            .get('/api/game/quizmasteringame')
            .then((response: AxiosResponse<QmInGameViewModel>) => {
                this.$store.commit('setTeamFeed', response.data.teamFeed);
                this.$store.commit('setGame', response.data.game);
            })
            .catch((error: AxiosError<ApiResponse>) => {
                this.$_helper_toastError(error);
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