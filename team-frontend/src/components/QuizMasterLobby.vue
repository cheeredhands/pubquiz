<template>
  <div id="main">
    <nav-bar-part>
      <b-nav-item>
        <b-button
          size="sm"
          v-if="game.state === runningState || game.state === pausedState"
          @click="startGame"
          variant="success"
          >{{ $t("CONTINUE_GAME") }}</b-button
        >
        <b-button size="sm" v-else @click="startGame" variant="success">
          <b-icon-play-fill v-if="game.state !== runningState" />{{
          $t("START_GAME")
        }}</b-button>
      </b-nav-item>
      <template v-slot:centercontent
        >Lobby - {{ game.title }} ({{ $t(game.state) }} {{ $t("SECTION") }}
        {{ game.currentSectionIndex }} :
        {{ game.currentQuizItemIndexInSection }}/{{
          game.currentSectionQuizItemCount
        }})</template
      >
    </nav-bar-part>
    <div class="main-container">
      <b-container>
        <b-row>
          <b-col lg="6">
            <b-card
              no-body
              class="example-drag mt-5"
              header-tag="header"
              :header="$t('OTHER_GAMES')"
            >
              <template #header>
                <span v-if="game.state === openState">
                  {{ $t("CURRENT_TEAMS_IN_LOBBY") }}
                </span>
                <span v-else>{{ $t("CURRENT_TEAMS_IN_GAME") }}</span>
              </template>
              <b-list-group flush>
                <b-list-group-item v-for="team in teams" :key="team.id">
                  <strong :title="team.recoveryCode">{{ team.name }} </strong>
                  <span class="small" v-if="team.memberNames"
                    >({{ team.memberNames }})</span
                  >&nbsp;
                  <b-badge
                    pill
                    :title="$t('NUMBER_OF_CONNECTIONS')"
                    variant="secondary"
                    v-if="team.connectionCount > 1"
                    >{{ team.connectionCount }}</b-badge
                  >
                  <b-badge v-if="!team.isLoggedIn">{{
                    $t("LOGGED_OUT")
                  }}</b-badge>
                  <b-icon-trash-fill
                    @click="kickTeam(team.id, team.name)"
                    class="float-right"
                    style="cursor: pointer"
                    :title="$t('KICK_OUT')"
                    v-b-tooltip
                  />
                </b-list-group-item>
              </b-list-group>
            </b-card>
          </b-col>
          <b-col lg="6">
            <b-card no-body class="example-drag mt-5" :header="$t('MY_GAMES')">
              <b-list-group flush>
                <b-list-group-item
                  v-for="gameRef in gameRefs.filter((r) => r.id === game.id)"
                  :key="gameRef.id"
                >
                  <strong>{{ gameRef.title }} </strong>
                  <span class="small">(quiz: {{ gameRef.quizTitle }})</span
                  >&nbsp;<code>{{ gameRef.inviteCode }}</code>
                  <b-icon-trash-fill
                    v-b-tooltip
                    @click="kickTeam(team.id, team.name)"
                    class="float-right"
                    style="cursor: pointer"
                    :title="$t('DELETE_GAME')"
                  />
                  <b-icon-pencil-fill
                    v-b-tooltip
                    :title="$t('EDIT_GAME')"
                    class="float-right mr-2"
                    style="cursor: pointer"
                    @click="kickTeam(gameRef.id)"
                  />
                </b-list-group-item> </b-list-group
            ></b-card>
            <b-card no-body class="example-drag mt-5" header-tag="header">
              <template #header>
                <span v-if="game.state === openState">
                  {{ $t("MY_QUIZZES") }}
                  <h5
                    class="float-right mb-0"
                    v-b-tooltip
                    :title="$t('ADD_QUIZ')"
                    style="cursor: pointer"
                  >
                    <b-icon-file-earmark-plus/>
                  </h5>
                </span>
                <span v-else>{{ $t("CURRENT_TEAMS_IN_GAME") }}</span>
              </template>
              <b-list-group flush>
                <b-list-group-item
                  v-for="quizRef in quizRefs"
                  :key="quizRef.id"
                >
                  <strong>{{ quizRef.title }} </strong>
                  <span class="small"
                    >{{ quizRef.gameRefs.length }} game<span
                      v-if="
                        quizRef.gameRefs.length === 0 ||
                        quizRef.gameRefs.length > 1
                      "
                      >s</span
                    > </span
                  >&nbsp;
                  <b-button
                    class="float-right"
                    variant="secondary"
                    size="sm"
                    pull="right"
                  >
                    {{ $t("ADD_GAME_FOR_QUIZ") }}
                  </b-button>
                </b-list-group-item>
              </b-list-group></b-card
            >
          </b-col>
        </b-row>
        <b-row
          ><b-col lg="6">
            <b-card class="example-drag mt-5" :header="$t('UPLOAD_QUIZ')">
              <div>
                <!-- Styled -->
                <b-form-file
                  accept="application/zip"
                  v-model="quizFile"
                  :state="Boolean(quizFile)"
                  placeholder="Choose a file or drop it here..."
                  drop-placeholder="Drop file here..."
                ></b-form-file>
                <div class="mt-3">
                  Selected file: {{ quizFile ? quizFile.name : "" }}
                </div>
                <b-button @click="uploadFile()">Upload</b-button>
              </div>
            </b-card>
          </b-col></b-row
        >
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
import { Game, Team, GameState, GameRef, QuizRef } from '../models/models';
import { ApiResponse } from '../models/apiResponses';
import QuizServiceMixin from '@/services/quiz-service-mixin';

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
  QuizServiceMixin,
  AccountServiceMixin,
  GameServiceMixin,
  HelperMixin
) {
  public name = 'QuizMasterLobby';
  public openState = GameState.Open;
  public runningState = GameState.Running;
  public pausedState = GameState.Paused;
  quizFile = null;

  public created(): void {
    this.$_gameService_getQmLobby().then(() => {
      document.title = 'Lobby - ' + this.game.title;
    });
  }

  public uploadFile(): void {
    if (this.quizFile !== undefined) {
      this.$_quizService_uploadQuiz(this.userId, this.quizFile as unknown as File)
        .catch((error: AxiosError<ApiResponse>) => {
          this.$_helper_toastError(error);
        });
    }
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

  get gameRefs(): GameRef[] {
    return this.$store.getters.gameRefs;
  }

  get quizRefs(): QuizRef[] {
    return this.$store.getters.quizRefs;
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.teamMembers {
  font-size: 0.8em;
}

.example-drag label.btn {
  margin-bottom: 0;
  margin-right: 1rem;
}
.example-drag .drop-active {
  top: 0;
  bottom: 0;
  right: 0;
  left: 0;
  position: fixed;
  z-index: 9999;
  opacity: 0.6;
  text-align: center;
  background: #000;
}
.example-drag .drop-active h3 {
  margin: -0.5em 0 0;
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  -webkit-transform: translateY(-50%);
  -ms-transform: translateY(-50%);
  transform: translateY(-50%);
  font-size: 40px;
  color: #fff;
  padding: 0;
}
</style>
