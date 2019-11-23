import Vue from 'vue'
import { AxiosResponse } from 'axios';

import { WhoAmIResponse, ApiResponse, RegisterForGameResponse, TeamLobbyViewModel, GameState, LoginResponse } from '../models/models';

import Component from 'vue-class-component';

@Component
export default class AccountServiceMixin extends Vue {
    public getWhoAmI(): Promise<AxiosResponse<WhoAmIResponse>> {
        return this.$axios.get('/api/account/whoami', { withCredentials: true });
    }
    public logOutCurrentUser(): Promise<AxiosResponse<ApiResponse>> {
        return this.$axios.post('/api/account/logout', { withCredentials: true });
    }
    public registerForGame(teamName: string, code: string): Promise<AxiosResponse<RegisterForGameResponse>> {
        return this.$axios.post('/api/account/register', { teamName, code }, { withCredentials: true });
    }
    public loginUser(userName: string, password: string): Promise<AxiosResponse<LoginResponse>> {
        return this.$axios.post('/api/account/login', { userName, password }, { withCredentials: true });
    }
    public getTeamLobby(): Promise<AxiosResponse<TeamLobbyViewModel>> {
        return this.$axios.get('/api/game/teamlobby', { withCredentials: true });
    }
    public setGameState(actorId: string, gameId: string, newGameState: GameState): Promise<AxiosResponse> {
        return this.$axios.post('api/game/setgamestate', {
            actorId, gameId, newGameState
        });
    }

    public deleteTeam(teamId: string): Promise<AxiosResponse> {
        return this.$axios.post('api/account/deleteteam', { teamId });
    }
}