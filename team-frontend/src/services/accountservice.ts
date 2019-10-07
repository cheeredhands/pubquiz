import Vue from 'vue'
import { AxiosResponse } from 'axios';
import { WhoAmIResponse, ApiResponse, RegisterForGameResponse, TeamLobbyViewModel } from '@/models/models';
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
        return this.$axios.post(
            '/api/account/register',
            {
                teamName,
                code
            },
            { withCredentials: true }
        )
    }
    public getTeamLobby(): Promise<AxiosResponse<TeamLobbyViewModel>> {
        return this.$axios.get('/api/game/teamlobby', { withCredentials: true });
    }
}