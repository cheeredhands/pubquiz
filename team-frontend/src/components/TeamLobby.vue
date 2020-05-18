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
          <b-form ref="teamNameForm" @submit="applyTeamNameChange" novalidate>
            <b-form-group
              :label="$t('TEAMNAME')"
              :description="$t('KEEP_IT_CLEAN')"
              label-for="nameInput"
            >
              <b-input-group>
                <font-awesome-icon
                  v-if="!teamNameEditable"                  
                  icon="pen"
                  @click="enterTeamNameEditMode"
                  :title="$t('EDIT')"
                />
                <b-form-input
                  :plaintext="!teamNameEditable"
                  @click="enterTeamNameEditMode"
                  @blur="exitTeamNameEditMode"
                  class="editable"
                  id="nameInput"
                  v-model="newName"
                  type="text"
                  name="nameInput"
                  required
                  size="lg"
                  minlength="5"
                  maxlength="30"
                ></b-form-input>
                <b-form-invalid-feedback>{{ $t('TEAMNAME_LENGTH') }}</b-form-invalid-feedback>
              </b-input-group>
            </b-form-group>
          </b-form>
          <b-form ref="memberNamesForm" @submit="saveMembers" novalidate>
            <b-form-group
              :label="$t('MEMBERS')"
              label-for="memberNamesInput"
              :description="$t('MEMBER_NAMES')"
            >
              <b-input-group>
                <font-awesome-icon
                  v-if="!memberNamesEditable"
                  style="display: inline-block;position:absolute; right:10px; top: 10px; z-index:10;"
                  icon="pen"
                  @click="enterMemberNamesEditMode"
                  :title="$t('EDIT')"
                />
                <b-form-textarea
                  :plaintext="!memberNamesEditable"
                  @click="enterMemberNamesEditMode"
                  @blur="exitMemberNamesEditMode"
                  class="editable"
                  :placeholder="$t('TEAM_MEMBERS_HERE')"
                  rows="5"
                  v-model="newMemberNames"
                  id="memberNamesInput"
                  name="membersNamesInput"
                  maxlength="140"
                ></b-form-textarea>
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
  public teamId: string = this.$store.getters.teamId;

  public newName: string = this.teamName;
  public newMemberNames: string = this.memberNames;
  public teamNameEditable = false;
  public memberNamesEditable = false;

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

  public enterTeamNameEditMode() {
    if (!this.teamNameEditable) {
      this.teamNameEditable = true;
    }
  }

  public enterMemberNamesEditMode() {
    if (!this.memberNamesEditable) {
      this.memberNamesEditable = true;
    }
  }

  public exitTeamNameEditMode(evt: Event) {
    this.applyTeamNameChange(evt);
  }

  public exitMemberNamesEditMode(evt: Event) {
    this.saveMembers(evt);
  }

  public saveMembers(evt: Event) {
    if (
      !this.$quizrhelpers.formIsValid(evt, this.$refs.memberNamesForm as any)
    ) {
      return;
    }
    if (this.memberNames === this.newMemberNames) {
      this.memberNamesEditable = false;
      return;
    }

    this.$axios
      .post('api/account/changeteammembers', {
        teamMembers: this.newMemberNames
      })
      .then((response: AxiosResponse<SaveTeamMembersResponse>) => {
        this.$store.commit('setOwnTeamMembers', response.data.teamMembers);
        this.newMemberNames = response.data.teamMembers;
        this.memberNamesEditable = false;
      })
      .catch((error: AxiosError<ApiResponse>) => {
        this.$_helper_toastError(error);
        this.newMemberNames = this.memberNames;
      });
  }

  public applyTeamNameChange(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt, this.$refs.teamNameForm as any)) {
      return;
    }

    // call api that team name changed but only if team name has not changed!
    if (this.teamName === this.newName) {
      this.teamNameEditable = false;
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

.fa-pen {
  cursor: pointer;
  display: inline-block;
  position: absolute;
  right: 15px;
  top: 15px;
  z-index: 10;
  color: lightgrey;
}

.input-group {
  border: 1px solid transparent;
  /* margin-left: 4px; */
}

.input-group:hover {
  border: 1px solid lightgrey;
}
.input-group:hover .fa-pen {
  /* display: inline-block; */
  color: black;
}
</style>
