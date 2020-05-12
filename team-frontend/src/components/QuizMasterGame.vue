
<template>
  <div id="app">
    <NavBarPart>
      <b-nav-item>
        <b-button
          @click="toggleGame"
          :variant="game.state===runningState ? 'secondary' : 'success' "
        >
          <font-awesome-icon :icon="game.state===runningState ? 'pause' : 'play'" />
          {{ game.state===runningState ? $t('PAUSE_GAME') : $t('RESUME_GAME') }}
        </b-button>&nbsp;
        <b-button @click="finishGame" variant="danger">
          <font-awesome-icon icon="power-off" />
          {{ $t('FINISH_GAME') }}
        </b-button>
      </b-nav-item>
      <template v-slot:centercontent>{{game.gameTitle}} ({{ $t(game.state) }})</template>
      <template v-slot:rightcontent>
        <b-nav-item to="/qm/lobby" :title="$t('LOBBY_TITLE')">Lobby</b-nav-item>
      </template>
    </NavBarPart>
    <div class="grid-container">
      <div class="teamfeed"><QmTeamFeedPart/></div>

      <div class="quiz-container">
        <div class="question">
          <QmQuestionPart />
        </div>

        <div class="ranking">[Ranking]</div>
      </div>
    </div>
    <FooterPart>Quiz master game screen footer</FooterPart>
  </div>
</template>


<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Route } from 'vue-router';
import store from '../store';
import { AxiosResponse, AxiosError } from 'axios';
import { ApiResponse, GameState } from '../models/models';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';
import QmQuestionPart from './qm-gameparts/QmQuestionPart.vue';
import QmTeamFeedPart from './qm-gameparts/QmTeamFeedPart.vue';
import { mixins } from 'vue-class-component';
import AccountServiceMixin from '../services/accountservice';

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
export default class QuizMasterGame extends mixins(AccountServiceMixin) {
  public name: string = 'QuizMasterGame';
  public runningState = GameState.Running;
  public created() {
    // // get QuizMaster Game view model
    // this.$axios
    //   .get("/api/game/quizmastergame")
    //   .then((response: AxiosResponse<QuizMasterGameViewModel>) => {
    //   })
    //   .catch((error: AxiosError) => {
    //     this.$bvToast.toast(error.message, {
    //       title: "oops",
    //       variant: "error"
    //     });
    //   });
  }

  get game() {
    return this.$store.getters.getGame;
  }

  get userId() {
    return this.$store.getters.getUserId;
  }

  public toggleGame() {
    this.setGameState(
      this.userId,
      this.game.gameId,
      this.game.state === GameState.Running
        ? GameState.Paused
        : GameState.Running
    )
      .then((response: AxiosResponse<ApiResponse>) => {
        // this.$router.push({ name: 'QuizMasterGame' });
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

  public finishGame() {
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
        this.setGameState(this.userId, this.game.gameId, GameState.Finished)
          .then(() => {
            // this.$router.push({ name: 'QuizMasterGame' });
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
      })
      .catch(err => {
        // An error occurred
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
  /* background-color: black;
  grid-gap: 1px; */
  grid-template-areas: "teamfeed quiz-container";
}

.grid-container > * {
  /* background-color: #fff; */
  border-right: 2px solid black;
  padding: 0.5em;
}
.teamfeed {
  grid-area: teamfeed;
}

.quiz-container {
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 4fr 3fr;
  grid-template-areas: "question" "ranking";
  grid-area: quiz-container;
  /* background-color: black;
  grid-gap: 1px; */
  padding: 0px;
}

.quiz-container > * {
  padding: 0.5em;
}

.question {
  grid-area: question;
  padding: 0px;
  border-bottom: 2px solid black;
}

.ranking {
  grid-area: ranking;
}
</style>
