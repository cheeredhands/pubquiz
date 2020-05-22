
<template>
  <div id="app">
    <nav-bar-part>
      <template v-slot:centercontent>{{game.title}} ({{ $t(game.state) }})</template>
      <template v-slot:rightcontent>
        <b-nav-item to="/lobby" :title="$t('LOBBY_TITLE')">Lobby</b-nav-item>
      </template>
    </nav-bar-part>
    <TeamQuestionPart />
    <footer-part>Team game screen footer</footer-part>
  </div>
</template>


<script lang="ts">
import Vue from 'vue';
import Component, { mixins } from 'vue-class-component';
import { Route } from 'vue-router';
import store from '../store';
import { AxiosResponse, AxiosError } from 'axios';
import { GameState } from '../models/models';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';
import TeamQuestionPart from './team-gameparts/TeamQuestionPart.vue';
import AccountServiceMixin from '../services/account-service-mixin';
import GameServiceMixin from '../services/game-service-mixin';
import HelperMixin from '../services/helper-mixin';
import { ApiResponse } from '../models/apiResponses';

@Component({
  components: { NavBarPart, FooterPart, TeamQuestionPart },
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
export default class TeamInGame extends mixins(
  AccountServiceMixin,
  GameServiceMixin,
  HelperMixin
) {
  public name: string = 'TeamInGame';
  public runningState = GameState.Running;
  public created() {
    this.$_gameService_getTeamInGame();
  }

  get game() {
    return this.$store.getters.game;
  }

}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
