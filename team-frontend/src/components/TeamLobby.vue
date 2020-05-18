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
          <b-form @submit="applyTeamNameChange" novalidate>
            <b-form-group
              :label="$t('TEAMNAME')"
              :description="$t('KEEP_IT_CLEAN')"
              label-for="nameInput"
            >
              <b-input-group>
                <!-- <font-awesome-icon
                  type="button"
                  style="display: inline-block;position:absolute; right:10px; top: 10px; z-index:10"
                  :icon="teamNameEditIcon"
                  :title="$t('EDIT')"
                />-->
                <b-form-input
                  :plaintext="!teamNameEditable"
                  class="editable"
                  @click="enterTeamNameEditMode"
                  id="nameInput"
                  v-model="newName"
                  type="text"
                  name="nameInput"
                  required
                  minlength="5"
                  maxlength="30"
                  @blur="exitTeamNameEditMode"
                ></b-form-input>

                <b-input-group-append>
                  <button type="submit">
                    <font-awesome-icon :icon="teamNameEditIcon" :title="$t('EDIT')" />
                  </button>
                </b-input-group-append>
                <b-form-invalid-feedback>{{ $t('TEAMNAME_LENGTH') }}</b-form-invalid-feedback>
              </b-input-group>
            </b-form-group>
          </b-form>
          <b-form @submit="saveMembers" novalidate>
            <b-form-group
              :label="$t('MEMBERS')"
              label-for="memberNamesInput"
              :description="$t('MEMBER_NAMES')"
            >
              <b-input-group>
                <b-form-textarea
                  rows="4"
                  v-model="newMemberNames"
                  id="memberNamesInput"
                  name="membersNamesInput"
                  maxlength="140"
                ></b-form-textarea>
                <b-input-group-append>
                  <b-button variant="primary" type="submit">{{ $t('SAVE')}}</b-button>
                </b-input-group-append>
              </b-input-group>
            </b-form-group>
          </b-form>
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
@Component({
  components: { NavBarPart, FooterPart },
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

  public newName: string = '';
  public teamNameEditable = false;
  public teamNameEditIcon = 'pen';
  public teamId: string = this.$store.getters.teamId;

  public newMemberNames: string = '';

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
    this.newName = this.teamName;
    this.newMemberNames = this.memberNames;
  }

  public enterTeamNameEditMode() {
    if (!this.teamNameEditable) {
      this.teamNameEditable = true;
    }
    this.teamNameEditIcon = 'check';
  }

  public exitTeamNameEditMode(evt: Event) {
    this.applyTeamNameChange(evt);
  }

  public saveMembers(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) {
      return;
    }
    if (this.memberNames === this.newMemberNames) {
      return;
    }

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

  public applyTeamNameChange(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) {
      // this.teamNameEditable = false;
      // this.teamNameEditIcon = 'pen';
      return;
    }

    // call api that team name changed but only if team name has not changed!
    if (this.teamName === this.newName) {
      this.teamNameEditable = false;
      this.teamNameEditIcon = 'pen';
      return;
    }

    this.$axios
      .post('/api/account/changeteamname', {
        teamId: this.teamId,
        newName: this.newName
      })
      .then((response: AxiosResponse<ApiResponse>) => {
        // only save it to the store if api call is successful!
        this.$store.commit('setOwnTeamName', this.newName);
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
      })
      .finally(() => {
        this.newName = this.teamName;
        this.teamNameEditable = false;
        this.teamNameEditIcon = 'pen';
      });
  }

  @Watch('isLoggedIn') public OnLoggedInChanged(
    value: boolean,
    oldValue: boolean
  ) {
    if (oldValue && !value) {
      // we've been kicked!
      this.$bvModal
        // Line below seen as error but worky!
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
button {
  background-color: Transparent;
  background-repeat: no-repeat;
  border: none;
  cursor: pointer;
  overflow: hidden;
  outline: none;
}
</style>
