<template>
  <div id="main">
    <nav-bar-part>
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
          <b-col>
            <code v-if="showRecoveryCode" class="float-right"
              >inlogcode: {{ recoveryCode }}</code
            >
            <h1 class="mt-3 mb-5" @click="showRecoveryCode = !showRecoveryCode">
              {{ $t("TEAMLOBBY_WELCOME") }}
            </h1>
            <p>{{ $t("TEAMLOBBY_SIT_BACK") }}</p>
          </b-col>
        </b-row>
        <b-row>
          <b-col md="6">
            <h3>{{ $t("YOUR_TEAM") }}</h3>
            <quizr-editable-textfield
              v-model="newName"
              :label="$t('TEAMNAME')"
              :description="$t('KEEP_IT_CLEAN')"
              :feedback="$t('TEAMNAME_LENGTH')"
              required
              :minlength="5"
              :maxlength="30"
              v-on:apply="applyTeamNameChange"
            ></quizr-editable-textfield>
            <quizr-editable-textarea
              v-model="newMemberNames"
              :label="$t('MEMBERS')"
              :placeholder="$t('MEMBER_NAMES')"
              :rows="5"
              :minlength="5"
              :maxlength="140"
              v-on:apply="saveMembers"
            ></quizr-editable-textarea>
          </b-col>
          <b-col>
            <h3>{{ $t("COMPETING_TEAMS") }}</h3>
            <b-list-group>
              <b-list-group-item
                class="d-flex justify-content-between align-items-center"
                v-for="otherTeam in teams"
                :key="otherTeam.id"
              >
                <div>
                  <h5 class="mt-0 mb-1">{{ otherTeam.name }}</h5>
                  <p class="mb-0 small">{{ otherTeam.memberNames }}</p>
                </div>
                <b-badge v-if="!otherTeam.isLoggedIn" pill>{{
                  $t("LOGGED_OUT")
                }}</b-badge>
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
import { Component, Watch } from 'vue-property-decorator';
import { mixins } from 'vue-class-component';
import { Route } from 'vue-router';
import store from '../store';
import { AxiosResponse, AxiosError } from 'axios';
import AccountServiceMixin from '../services/account-service-mixin';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';
import HelperMixin from '../services/helper-mixin';
import GameServiceMixin from '../services/game-service-mixin';
import { ApiResponse, SaveTeamMembersResponse } from '../models/apiResponses';
import QuizrEditableTextfield from './controls/QuizrEditableTextfield.vue';
import QuizrEditableTextarea from './controls/QuizrEditableTextarea.vue';
import { GameState, Game } from '../models/models';
import { TeamViewModel } from '../models/viewModels';

@Component({
  components: {
    NavBarPart,
    FooterPart,
    QuizrEditableTextfield,
    QuizrEditableTextarea
  },
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
export default class TeamLobby extends mixins(
  AccountServiceMixin,
  GameServiceMixin,
  HelperMixin
) {
  public name = 'TeamLobby';
  public teamId: string = this.$store.getters.teamId;
  public showRecoveryCode = false;
  public newName: string = this.teamName;
  public newMemberNames: string = this.memberNames;

  get isLoggedIn(): boolean {
    return this.$store.state.isLoggedIn || false;
  }

  get recoveryCode(): string {
    return this.$store.getters.recoveryCode || '';
  }

  get game(): Game {
    return (this.$store.state.game || {}) as Game;
  }

  get gameState(): GameState {
    return this.game.state;
  }

  get teamName(): string {
    return this.$store.getters.teamName;
  }

  get memberNames(): string {
    return this.$store.getters.memberNames;
  }

  get teams(): TeamViewModel[] {
    return (this.$store.state.teams || []) as TeamViewModel[];
  }

  public created(): void {
    this.$_gameService_getTeamLobby();
  }

  public saveMembers(): void {
    this.$axios
      .post('api/account/changeteammembers', {
        teamMembers: this.newMemberNames
      })
      .then((response: AxiosResponse<SaveTeamMembersResponse>) => {
        this.$store.commit('setOwnTeamMembers', response.data.teamMembers);
        this.newMemberNames = response.data.teamMembers;
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
        this.newMemberNames = this.memberNames;
      });
  }

  public applyTeamNameChange(): void {
    this.$axios
      .post('/api/account/changeteamname', {
        teamId: this.teamId,
        newName: this.newName
      })
      .then(() => {
        this.$store.commit('setOwnTeamName', this.newName);
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      })
      .finally(() => {
        this.newName = this.teamName;
      });
  }

  @Watch('gameState')
  public OnGameStateChanged(value: GameState): void {
    if (value === GameState.Running) {
      this.$router.replace({ name: 'TeamInGame' });
    }
  }

  @Watch('isLoggedIn') public OnLoggedInChanged(
    value: boolean,
    oldValue: boolean
  ): void {
    if (oldValue && !value) {
      this.$bvModal
        .msgBoxOk(this.$t('KICKED_OUT').toString(), {
          title: this.$t('REMOVED').toString(),
          centered: true
        })
        .then(() => {
          this.$router.push({ name: 'RegisterTeam' });
        });
    }
  }
}
</script>
