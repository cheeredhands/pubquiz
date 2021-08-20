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
          }}</b-button
        >
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
            <b-card no-body class="mt-5" :header="$t('CURRENT_GAME')">
              <b-list-group flush>
                <b-list-group-item>
                  <quizr-editable-textfield
                    v-model="game.title"
                    label=""
                    description=""
                    :feedback="$t('GAMETITLE_FEEDBACK')"
                    required
                    :minlength="5"
                    :maxlength="30"
                    v-on:apply="applyGameTitleChange"
                  ></quizr-editable-textfield>
                  <b-badge class="float-right" pill variant="success">{{
                    $t(game.state)
                  }}</b-badge>
                  <quizr-inline-editable-textfield
                    v-model="game.inviteCode"
                    label="Code"
                    description=""
                    :feedback="$t('INVITECODE_FEEDBACK')"
                    required
                    :minlength="5"
                    :maxlength="30"
                    v-on:apply="applyInviteCodeChange"
                  ></quizr-inline-editable-textfield>
                  <span class="small"
                    ><strong>quiz:</strong> {{ game.quizTitle }}</span
                  ></b-list-group-item
                >
              </b-list-group></b-card
            >
            <b-card no-body class="mt-5" header-tag="header">
              <template #header>
                <div class="collapser" v-b-toggle.collapse-my-games>
                  {{ $t("MY_GAMES") }}
                  <b-icon-caret-down-fill class="float-right collapse-icon" />
                </div>
              </template>
              <b-collapse id="collapse-my-games">
                <b-list-group flush>
                  <b-list-group-item
                    :class="{ 'selected-game': gameVm.id === game.id }"
                    v-for="gameVm in gameViewModels"
                    :key="gameVm.id"
                  >
                    <quizr-editable-textfield
                      v-model="gameVm.title"
                      label=""
                      description=""
                      :feedback="$t('GAMETITLE_FEEDBACK')"
                      required
                      :minlength="5"
                      :maxlength="30"
                      v-on:apply="applyGameTitleChange"
                    ></quizr-editable-textfield>
                    <b-badge class="float-right" pill variant="success">{{
                      $t(gameVm.gameState)
                    }}</b-badge>
                    <quizr-inline-editable-textfield
                      v-model="gameVm.inviteCode"
                      label="Code"
                      description=""
                      :feedback="$t('INVITECODE_FEEDBACK')"
                      required
                      :minlength="5"
                      :maxlength="30"
                      v-on:apply="applyInviteCodeChange"
                    ></quizr-inline-editable-textfield>
                    <span class="small"
                      ><strong>quiz:</strong> {{ gameVm.quizTitle }}</span
                    >
                    <!-- <b-icon-trash-fill
                    v-b-tooltip
                    @click="finishGame(gameVm.id)"
                    class="float-right"
                    style="cursor: pointer"
                    :title="$t('FINISH_GAME')"
                  /> -->
                    <b-button
                      v-if="gameVm.id !== game.id"
                      v-b-tooltip
                      :title="$t('SELECT_GAME')"
                      class="float-right mr-2"
                      size="sm"
                      variant="secondary"
                      @click="selectGame(gameVm.id)"
                    >
                      <b-icon-check-circle
                    /></b-button>
                  </b-list-group-item> </b-list-group></b-collapse
            ></b-card>
            <b-card no-body class="mt-3" header-tag="header">
              <template #header>
                <div class="collapser" v-b-toggle.collapse-my-quizzes>
                  {{ $t("MY_QUIZZES") }}
                  <b-icon-caret-down-fill class="float-right collapse-icon" />
                </div>
              </template>
              <b-collapse id="collapse-my-quizzes">
                <b-list-group flush>
                  <b-list-group-item
                    v-for="quizVm in quizViewModels"
                    :key="quizVm.id"
                  >
                    <strong>{{ quizVm.title }} </strong>
                    <span class="small"
                      >{{
                        gameViewModels.filter((g) => g.quizId == quizVm.id)
                          .length
                      }}
                      <span
                        v-if="
                          gameViewModels.filter((g) => g.quizId == quizVm.id)
                            .length === 1
                        "
                        >{{ $t("GAME") }}</span
                      ><span v-else>{{ $t("GAMES") }} </span>
                    </span>
                    <b-button
                      v-b-toggle="[`addgame-${quizVm.id}`]"
                      class="float-right collapser"
                      variant="secondary"
                      size="sm"
                    >
                      {{ $t("ADD_GAME_FOR_QUIZ") }}
                      <b-icon-caret-down-fill class="collapse-icon" />
                    </b-button>
                    <b-collapse :id="`addgame-${quizVm.id}`">
                      <br />
                      <b-form inline>
                        <b-input-group size="sm" prepend="Titel">
                          <b-form-input
                            :id="`addgame-title-${quizVm.id}`"
                            placeholder="Titel"
                            size="sm"
                          >
                          </b-form-input
                        ></b-input-group>
                        <b-input-group size="sm" prepend="Code" class="ml-1">
                          <b-form-input
                            :id="`addgame-code-${quizVm.id}`"
                            placeholder="Code"
                            size="sm"
                          >
                          </b-form-input
                        ></b-input-group>
                        <b-button
                          @click="addGameForQuiz(quizVm.id)"
                          class="ml-1 float-right"
                          variant="primary"
                          size="sm"
                        >
                          <b-icon-check-circle />
                        </b-button>
                      </b-form>
                    </b-collapse>
                  </b-list-group-item>
                  <b-list-group-item>
                    <h5>{{ $t("UPLOAD_QUIZ") }}</h5>
                    <div>
                      <!-- Styled -->
                      <b-form-file
                        accept="application/zip"
                        v-model="quizFile"
                        :state="Boolean(quizFile)"
                        placeholder="Choose a file or drop it here..."
                        drop-placeholder="Drop file here..."
                      ></b-form-file>
                      <b-button class="mt-3 float-right" @click="uploadFile()"
                        >Upload</b-button
                      >
                      <div class="mt-3">
                        Selected file: {{ quizFile ? quizFile.name : "" }}
                      </div>
                    </div>
                  </b-list-group-item>
                </b-list-group></b-collapse
              ></b-card
            >
          </b-col>
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
import QuizServiceMixin from '@/services/quiz-service-mixin';
import QuizrEditableTextfield from './controls/QuizrEditableTextfield.vue';
import QuizrInlineEditableTextfield from './controls/QuizrInlineEditableTextfield.vue';
import { GameViewModel, QuizViewModel } from '@/models/viewModels';

@Component({
  components: { NavBarPart, FooterPart, QuizrEditableTextfield, QuizrInlineEditableTextfield },
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

  public applyGameTitleChange(): void {
    alert('hi');
  }

  public applyInviteCodeChange(): void {
    alert('hi');
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
        });
    } else {
      this.$router.push({ name: 'QuizMasterInGame' });
    }
  }

  public selectGame(gameId: string): void {
    this.$_gameService_selectGame(gameId);
  }

  public finishGame(gameId: string): void {
    this.$bvModal
      .msgBoxConfirm(this.$t('CONFIRM_FINISH_GAME').toString(), {
        title: this.$t('PLEASE_CONFIRM').toString(),
        okVariant: 'danger'.toString(),
        okTitle: this.$t('YES').toString(),
        cancelTitle: this.$t('NO').toString()
      })
      .then(value => {
        if (!value) {
          return;
        }
        this.$_gameService_setGameState(this.userId, gameId, GameState.Finished);
      });
  }

  public addGameForQuiz(quizId: string): void {
    const title = (document.getElementById(`addgame-title-${quizId}`) as HTMLInputElement).value;
    const code = (document.getElementById(`addgame-code-${quizId}`) as HTMLInputElement).value;
    if (title === null || code === null) return;
    this.$_quizService_addGameForQuiz(this.userId, quizId, title, code);
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

  get gameViewModels(): GameViewModel[] {
    return this.$store.getters.gameViewModels;
  }

  get quizViewModels(): QuizViewModel[] {
    return this.$store.getters.quizViewModels;
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

/* .collapsed > .when-open,
.not-collapsed > .when-closed {
  display: none;
} */

.card-header .collapse-icon {
  transition: 0.3s transform ease-in-out;
}
.card-header.collapsed .collapse-icon {
  transform: rotate(90deg);
}

.collapser .collapse-icon {
  transition: 0.3s transform ease-in-out;
}
.collapser.collapsed .collapse-icon {
  transform: rotate(90deg);
}

.selected-game {
  background-color: lightgreen;
}
</style>
