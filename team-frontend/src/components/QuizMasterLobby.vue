<template>
  <div id="app">
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
              <b-list-group-item v-for="(team, index) in teams" :key="index">
                <strong>{{ team.teamName }}</strong> -
                <span>{{team.memberNames}}</span>&nbsp;
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
    </div>
    <footer-part></footer-part>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Route } from 'vue-router';
import store from '../store';
import { AxiosResponse, AxiosError } from 'axios';
import { mixins } from 'vue-class-component';
import AccountServiceMixin from '../services/account-service-mixin';
import GameServiceMixin from '../services/game-service-mixin';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';
import HelperMixin from '../services/helper-mixin';
import { GameState } from '../models/models';
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
  public name: string = 'QuizMasterLobby';
  public openState = GameState.Open;
  public runningState = GameState.Running;
  public pausedState = GameState.Paused;

  public created() {
    this.$_gameService_getQmLobby();
    document.title = 'Lobby - ' + this.game.title;
  }

  // public mounted() {
  //   document.title = this.game.title;
  // }

  public startGame() {
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

  public kickTeam(teamId: string, teamName: string) {
    this.$_accountService_deleteTeam(teamId)
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
        this.$_helper_toastError(error);
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
