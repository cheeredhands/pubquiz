import Vue from 'vue';
import { AxiosResponse } from 'axios';
import Component from 'vue-class-component';
import { WhoAmIResponse, ApiResponse, RegisterForGameResponse, LoginResponse } from '../models/apiResponses';
/* eslint camelcase: "off" */
@Component
export default class AccountServiceMixin extends Vue {
  public $_accountService_getWhoAmI(): Promise<AxiosResponse<WhoAmIResponse>> {
    return this.$axios.get('/api/account/whoami', { withCredentials: true });
  }

  public $_accountService_logOutCurrentUser(): Promise<AxiosResponse<ApiResponse>> {
    return this.$axios.post('/api/account/logout', { withCredentials: true });
  }

  public $_accountService_registerForGame(name: string, code: string): Promise<AxiosResponse<RegisterForGameResponse>> {
    return this.$axios.post('/api/account/registerteam', { name, code }, { withCredentials: true });
  }

  public $_accountService_loginUser(userName: string, password: string): Promise<AxiosResponse<LoginResponse>> {
    return this.$axios.post('/api/account/login', { userName, password }, { withCredentials: true });
  }

  public $_accountService_deleteTeam(teamId: string): Promise<AxiosResponse> {
    return this.$axios.delete(`api/team/${teamId}`);
  }
}
