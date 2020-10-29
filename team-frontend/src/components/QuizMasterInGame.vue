
<template>
  <div id="app">
    <nav-bar-part>
      <b-nav-item>
        <b-button
          @click="toggleGame"
          :variant="game.state === runningState ? 'secondary' : 'success'"
        >
          <font-awesome-icon
            :icon="game.state === runningState ? 'pause' : 'play'"
          />
          {{
            game.state === runningState ? $t("PAUSE_GAME") : $t("RESUME_GAME")
          }} </b-button
        >&nbsp;
        <b-button @click="finishGame" variant="danger">
          <font-awesome-icon icon="power-off" />
          {{ $t("FINISH_GAME") }}
        </b-button>
      </b-nav-item>
      <template v-slot:centercontent
        >{{ game.title }} (Quiz: '{{ game.quizTitle }}'
        {{ $t(game.state) }})</template
      >
      <template v-slot:rightcontent>
        <b-nav-item to="/qm/lobby" :title="$t('LOBBY_TITLE')">Lobby</b-nav-item>
      </template>
    </nav-bar-part>

    <div class="grid-container">
      <div class="teamfeed">
        <qm-team-feed-part />
      </div>

      <div class="quiz-container">
        <div class="question">
          <QmQuestionPart />
        </div>

        <div class="ranking">
          <ul
            class="list-unstyled"
            v-for="team in qmTeamsSorted"
            :key="team.id"
          >
            <b-media tag="li">
              <template v-slot:aside>
                <h1 :title="$t('TOTAL_NUMBER_OF_POINTS')">
                  {{ team.totalScore }}
                </h1>
                <!-- <b-img blank blank-color="#abc" width="64" alt="placeholder"></b-img> -->
              </template>
              <h5 class="mt-0 mb-1">{{ team.name }}</h5>
              <p class="mb-0">
                TODO: score in this quiz section, trend (going up or sinking).
              </p>
            </b-media>
          </ul>
        </div>
      </div>
    </div>

    <footer-part>Quiz master game screen footer</footer-part>
  </div>
</template>

<script lang="ts">
import Component, { mixins } from 'vue-class-component';
import { Route } from 'vue-router';
import store from '../store';
import { Game, GameState, Team } from '../models/models';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';
import QmQuestionPart from './qm-gameparts/QmQuestionPart.vue';
import QmTeamFeedPart from './qm-gameparts/QmTeamFeedPart.vue';
import AccountServiceMixin from '../services/account-service-mixin';
import GameServiceMixin from '../services/game-service-mixin';
import HelperMixin from '../services/helper-mixin';

@Component({
  components: { NavBarPart, FooterPart, QmQuestionPart, QmTeamFeedPart },
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
export default class QuizMasterInGame extends mixins(
  AccountServiceMixin,
  GameServiceMixin,
  HelperMixin
) {
  public name = 'QuizMasterInGame';
  public runningState = GameState.Running;
  public async created(): Promise<void> {
    await this.$_gameService_getQmInGame();
    document.title = 'In Game - ' + this.game.title;
  }

  get game(): Game {
    return (this.$store.getters.game || {}) as Game;
  }

  get qmTeams(): Team[] {
    return (this.$store.getters.qmTeams || []) as Team[];
  }

  get qmTeamsSorted(): Team[] {
    return this.$store.getters.qmTeams.sort((a: Team, b: Team) => b.totalScore - a.totalScore);
  }

  get userId(): string {
    return this.$store.getters.userId;
  }

  public toggleGame(): void {
    this.$_gameService_setGameState(
      this.userId,
      this.game.id,
      this.game.state === GameState.Running
        ? GameState.Paused
        : GameState.Running
    );
  }

  public finishGame(): void {
    this.$bvModal
      .msgBoxConfirm(this.$t('CONFIRM_END_GAME').toString(), {
        title: this.$t('PLEASE_CONFIRM').toString(),
        okVariant: 'danger'.toString(),
        okTitle: this.$t('YES').toString(),
        cancelTitle: this.$t('NO').toString()
      })
      .then(value => {
        if (!value) {
          return;
        }
        this.$_gameService_setGameState(
          this.userId,
          this.game.id,
          GameState.Finished
        );
      });
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.grid-container {
  display: grid;
  grid-template-columns: 5fr 6fr;
  grid-template-rows: 1fr;
  grid-template-areas: "teamfeed quiz-container";
  overflow: hidden;
}

.grid-container > * {
  border-right: 4px solid #212529;
  padding: 0.5em;
}
.teamfeed {
  grid-area: teamfeed;
  padding: 0px;
  overflow: hidden;
}

.quiz-container {
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 4fr 3fr;
  grid-template-areas: "question" "ranking";
  grid-area: quiz-container;
  padding: 0px;
  overflow: hidden;
}

.quiz-container > * {
  padding: 0.5em;
}

.question {
  grid-area: question;
  padding: 0px;
  border-bottom: 4px solid #212529;
}

.ranking {
  grid-area: ranking;
  overflow: auto;
}
</style>
