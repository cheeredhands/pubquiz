
<template>
  <div id="main">
    <nav-bar-part>
      <template v-slot:centercontent>{{game.quizTitle}} ({{ $t(game.state) }})</template>
      <template v-slot:rightcontent>
        <b-nav-item to="/lobby" :title="$t('LOBBY_TITLE')">Lobby</b-nav-item>
      </template>
    </nav-bar-part>
    <TeamQuestionPart />
    <footer-part>Team game screen footer</footer-part>
    <b-modal
      size="xl"
      v-model="isPaused"
      id="modalPaused"
      header-bg-variant="warning"
      centered
      no-close-on-backdrop
      no-close-on-esc
      :title="$t('Paused')"
    >
      <template v-slot:modal-header>
        <h1>{{$t('Paused')}}</h1>
      </template>
      <template v-slot:default>
        <p class="my-4">{{$t('THE_GAME_IS_PAUSED')}}</p>
      </template>
      <template v-slot:modal-footer>{{$t('WAIT_FOR_RESUME')}}</template>
    </b-modal>
  </div>
</template>

<script lang="ts">
import Component, { mixins } from 'vue-class-component';
import { Route } from 'vue-router';
import store from '../store';
import { Game, GameState } from '../models/models';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';
import TeamQuestionPart from './team-gameparts/TeamQuestionPart.vue';
import AccountServiceMixin from '../services/account-service-mixin';
import GameServiceMixin from '../services/game-service-mixin';
import HelperMixin from '../services/helper-mixin';
import { Watch } from 'vue-property-decorator';

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
  public name = 'TeamInGame';
  public runningState = GameState.Running;

  public created(): void {
    this.$_gameService_getTeamInGame();
  }

  get game(): Game {
    return (this.$store.getters.game || {}) as Game;
  }

  get gameState(): GameState {
    return this.game.state;
  }

  get isPaused(): boolean {
    return this.gameState === GameState.Paused || this.gameState === GameState.Reviewing;
  }

  @Watch('gameState')
  public OnGameStateChanged(value: GameState): void {
    if (value === GameState.Finished) {
      // todo route to end
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
body.modal-open #main {
  -webkit-filter: blur(12px);
  -moz-filter: blur(12px);
  -o-filter: blur(12px);
  -ms-filter: blur(12px);
  filter: blur(12px);
  filter: progid:DXImageTransform.Microsoft.Blur(PixelRadius='12');
}
</style>
