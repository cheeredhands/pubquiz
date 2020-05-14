import Vue from 'vue'
import { AxiosResponse } from 'axios';

import { WhoAmIResponse, ApiResponse, RegisterForGameResponse, TeamLobbyViewModel, GameState, LoginResponse } from '../models/models';

import Component from 'vue-class-component';

@Component
export default class GameServiceMixin extends Vue {

    public $_gameService_setGameState(actorId: string, gameId: string, newGameState: GameState): Promise<AxiosResponse> {
        return this.$axios.post('api/game/setgamestate', {
            actorId, gameId, newGameState
        });
    }

    public $_gameService_navigateItem(gameId: string, offset: number): Promise<AxiosResponse> {
        return this.$axios.post('api/game/navigate',{
            gameId, offset
        });
    }
}