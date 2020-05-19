<template>
  <div id="app">
    <NavBarPart />
    <b-container>
      <b-row>
        <b-col>
          <h1>{{ $t('TEAMLOBBY_WELCOME')}}</h1>
          <p>{{ $t('TEAMLOBBY_SIT_BACK')}}</p>
          <hr />
        </b-col>
      </b-row>
      <b-row>
        <b-col md="6">
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
            :description="$t('MEMBER_NAMES')"
            :placeholder="$t('TEAM_MEMBERS_HERE')"
            :rows="5"
            :minlength="5"
            :maxlength="140"
            v-on:apply="saveMembers"
          ></quizr-editable-textarea>
        </b-col>
        <b-col>
          <p>{{ $t('COMPETING_TEAMS')}}</p>
          <b-list-group>
            <b-list-group-item
              v-for="(otherTeam, index) in teams"
              :key="index"
              :title="otherTeam.memberNames"
            >
              {{ otherTeam.teamName }}
              <span v-if="!otherTeam.isLoggedIn">{{ $t('LOGGED_OUT')}}</span>
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
  public name: string = 'TeamLobby';
  public teamId: string = this.$store.getters.teamId;

  public newName: string = this.teamName;
  public newMemberNames: string = this.memberNames;

  get isLoggedIn(): boolean {
    return this.$store.state.isLoggedIn || false;
  }

  get teamName() {
    return this.$store.getters.teamName;
  }

  get memberNames() {
    return this.$store.getters.memberNames;
  }

  get teams() {
    return this.$store.state.teams || [];
  }

  public created() {
    this.$_gameService_getTeamLobby();
  }

  public saveMembers() {
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

  public applyTeamNameChange() {
    this.$axios
      .post('/api/account/changeteamname', {
        teamId: this.teamId,
        newName: this.newName
      })
      .then((response: AxiosResponse<ApiResponse>) => {
        this.$store.commit('setOwnTeamName', this.newName);
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      })
      .finally(() => {
        this.newName = this.teamName;
      });
  }

  @Watch('isLoggedIn') public OnLoggedInChanged(
    value: boolean,
    oldValue: boolean
  ) {
    if (oldValue && !value) {
      this.$bvModal
        .msgBoxOk(this.$t('KICKED_OUT').toString(), {
          title: this.$t('REMOVED').toString(),
          centered: true
        })
        .then(_ => {
          this.$router.push({ name: 'RegisterTeam' });
        });
    }
  }
}
</script>

<style scoped>
</style>
