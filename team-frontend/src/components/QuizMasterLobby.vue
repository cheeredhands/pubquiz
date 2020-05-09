<template>
  <div id="app">
    <NavBarPart>
      <template v-slot:titlecontent>Lobby</template>
    </NavBarPart>
    <b-container>
      <b-row>
        <b-col>
          Game title: {{game.gameTitle}} (state: {{game.state}})
          <b-button
            v-if="game.state===runningState || game.state===pausedState"
            @click="startGame"
            variant="success"
          >{{ $t('CONTINUE_GAME') }}</b-button>
          <b-button
            v-if="game.state===openState"
            @click="startGame"
            variant="success"
          >{{ $t('START_GAME') }}</b-button>
          <hr />
        </b-col>
      </b-row>
      <b-row>
        <b-col lg="6">
          <p v-if="game.state===openState">{{ $t('CURRENT_TEAMS_IN_LOBBY')}}</p>
          <p v-else>{{ $t('CURRENT_TEAMS_IN_GAME')}}</p>
          <b-list-group>
            <b-list-group-item v-for="(team, index) in teams" :key="index">
              <strong>{{ team.teamName }}</strong> -
              <span class="teamMembers">{{team.memberNames}}</span>&nbsp;
              <b-badge v-if="!team.isLoggedIn">{{ $t('LOGGED_OUT') }}</b-badge>
              <font-awesome-icon
                icon="trash-alt"
                @click="kickTeam(team.teamId, team.teamName)"
                pull="right"
                style="cursor:pointer;"
                :title="$t('KICK_OUT')"
              />
            </b-list-group-item>
          </b-list-group>
        </b-col>
      </b-row>
    </b-container>
    <FooterPart />
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Route } from 'vue-router';
import store from '../store';
import { AxiosResponse, AxiosError } from 'axios';
import { mixins } from 'vue-class-component';
import AccountServiceMixin from '../services/accountservice';
import {
  QuizMasterLobbyViewModel,
  ApiResponse,
  GameState,
  GameStateChanged
} from '../models/models';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';

@Component({
  components: { NavBarPart, FooterPart },
  beforeRouteEnter(to: Route, from: Route, next: any) {
    // called before the route that renders this component is confirmed.
    // does NOT have access to `this` component instance,
    // because it has not been created yet when this guard is called!

    if (!store.state.isLoggedIn) {
      next('/');
    }
    // todo also check the state of the game, you might want to go straight back into the game.
    next();
  }
})
export default class QuizMasterLobby extends mixins(AccountServiceMixin) {
  public name: string = 'QuizMasterLobby';
  public openState = GameState.Open;
  public runningState = GameState.Running;
  public pausedState = GameState.Paused;
  public created() {
    this.$store.commit('setNavbarText', 'Quiz master lobby');
    // get team lobby view model
    this.$axios
      .get('/api/game/quizmasterlobby')
      .then((response: AxiosResponse<QuizMasterLobbyViewModel>) => {
        this.$store.commit('setTeams', response.data.teamsInGame);
        this.$store.commit('setGame', response.data.currentGame);
      })
      .catch((error: AxiosError<ApiResponse>) => {
        const errorCode =
          error !== undefined && error.response !== undefined
            ? error.response.data.code
            : 'UNKNOWN_ERROR';
        this.$bvToast.toast(this.$t(errorCode).toString(), {
          title: this.$t('ERROR_MESSAGE_TITLE').toString(),
          variant: 'error'
        });
      });
  }

  public startGame() {
    if (this.game.state === GameState.Open) {
      this.setGameState(this.userId, this.game.gameId, GameState.Running)
        .then(() => {
          this.$router.push({ name: 'QuizMasterGame' });
        })
        .catch((error: AxiosError<ApiResponse>) => {
          const errorCode =
            error !== undefined && error.response !== undefined
              ? error.response.data.code
              : 'UNKNOWN_ERROR';
          this.$bvToast.toast(this.$t(errorCode).toString(), {
            title: this.$t('ERROR_MESSAGE_TITLE').toString(),
            variant: 'error'
          });
        });
    } else {
      this.$router.push({ name: 'QuizMasterGame' });
    }
  }

  public kickTeam(teamId: string, teamName: string) {
    this.deleteTeam(teamId)
      .then(() => {
        this.$bvToast.toast(
          this.$t('TEAM_KICKED_OUT', { teamName }).toString(),
          {
            title: this.$t('REMOVED').toString(),
            variant: 'warning'
          }
        );
      })
      .catch((error: AxiosError<ApiResponse>) => {
        const errorCode =
          error !== undefined && error.response !== undefined
            ? error.response.data.code
            : 'UNKNOWN_ERROR';
        this.$bvToast.toast(this.$t(errorCode).toString(), {
          title: this.$t('ERROR_MESSAGE_TITLE').toString(),
          variant: 'error'
        });
      });
  }

  public messageTeam() {
    this.$bvToast.toast('todo: send message to team', {
      title: 'todo',
      variant: 'warning'
    });
  }

  get teams() {
    return this.$store.state.teams || [];
  }

  get game() {
    return this.$store.state.game || {};
  }

  get userName() {
    return this.$store.state.user.userName || '';
  }

  get userId() {
    return this.$store.state.userId || '';
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.teamMembers {
  font-size: 0.8em;
}
</style>
