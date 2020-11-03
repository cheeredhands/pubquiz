<template>
  <div id="main">
    <nav-bar-part>
      <b-nav-item>
        <b-button
          v-if="game.state===runningState || game.state===pausedState"
          @click="startGame"
          variant="success"
        >{{ $t('CONTINUE_GAME') }}</b-button>
        <b-button v-else @click="startGame" variant="success">{{ $t('START_GAME') }}</b-button>
      </b-nav-item>
      <template
        v-slot:centercontent
      >Lobby - {{game.title}} ({{ $t(game.state) }} {{$t('SECTION')}} {{game.currentSectionIndex}} : {{game.currentQuizItemIndexInSection}}/{{game.currentSectionQuizItemCount}})</template>
    </nav-bar-part>
    <div class="main-container">
      <b-container>
        <b-row>
          <b-col lg="6">
            <p v-if="game.state===openState">{{ $t('CURRENT_TEAMS_IN_LOBBY')}}</p>
            <p v-else>{{ $t('CURRENT_TEAMS_IN_GAME')}}</p>
            <b-list-group>
              <b-list-group-item v-for="team in teams" :key="team.id">
                <strong :title="team.recoveryCode">{{ team.name }} </strong>
                <span class="teamMembers" v-if="team.memberNames">({{team.memberNames}})</span>&nbsp;
                <b-badge pill :title="$t('NUMBER_OF_CONNECTIONS')" variant="secondary" v-if="team.connectionCount>1" >{{team.connectionCount}}</b-badge>
                <b-badge v-if="!team.isLoggedIn">{{ $t('LOGGED_OUT') }}</b-badge>
                <font-awesome-icon
                  icon="trash-alt"
                  @click="kickTeam(team.id, team.name)"
                  pull="right"
                  style="cursor:pointer;"
                  :title="$t('KICK_OUT')"
                />
              </b-list-group-item>
            </b-list-group>
          </b-col>
        </b-row>
      </b-container>
    </div>
    <footer-part></footer-part>
  </div>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator';
import { Route } from 'vue-router';
import store from '../store';
import { AxiosError } from 'axios';
import { mixins } from 'vue-class-component';
import AccountServiceMixin from '../services/account-service-mixin';
import GameServiceMixin from '../services/game-service-mixin';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';
import HelperMixin from '../services/helper-mixin';
import { Game, Team, GameState } from '../models/models';
import { ApiResponse } from '../models/apiResponses';

@Component({
  components: { NavBarPart, FooterPart },
  beforeRouteEnter(to: Route, from: Route, next: any) {
    // called before the route that renders this component is confirmed.
    // does NOT have access to `this` component instance,
    // because it has not been created yet when this guard is called!

    if (!store.state.isLoggedIn) {
      next('/');
    }
    next();
  }
})
export default class QuizMasterLobby extends mixins(
  AccountServiceMixin,
  GameServiceMixin,
  HelperMixin
) {
  public name = 'QuizMasterLobby';
  public openState = GameState.Open;
  public runningState = GameState.Running;
  public pausedState = GameState.Paused;

  public created(): void {
    this.$_gameService_getQmLobby().then(() => {
      document.title = 'Lobby - ' + this.game.title;
    });
  }

  public startGame(): void {
    if (this.game.state === GameState.Open) {
      this.$_gameService_setGameState(
        this.userId,
        this.game.id,
        GameState.Running
      )
        .then(() => {
          this.$router.push({ name: 'QuizMasterInGame' });
        })
        .catch((error: AxiosError<ApiResponse>) => {
          this.$_helper_toastError(error);
        });
    } else {
      this.$router.push({ name: 'QuizMasterInGame' });
    }
  }

  public kickTeam(teamId: string, name: string): void {
    this.$_accountService_deleteTeam(teamId)
      .then(() => {
        this.$bvToast.toast(this.$t('TEAM_KICKED_OUT', { teamName: name }).toString(), {
          title: this.$t('REMOVED').toString(),
          variant: 'warning'
        });
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      });
  }

  public messageTeam(): void {
    this.$bvToast.toast('todo: send message to team', {
      title: 'todo',
      variant: 'warning'
    });
  }

  get teams(): Team[] {
    return this.$store.state.qmTeams;
  }

  get game(): Game {
    return this.$store.getters.game as Game;
  }

  get userName(): string {
    return this.$store.getters.userName;
  }

  get userId(): string {
    return this.$store.getters.userId;
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.teamMembers {
  font-size: 0.8em;
}
</style>
