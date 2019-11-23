<template>
  <b-container fluid>
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
              <b-form-input
                id="nameInput"
                v-model="newName"
                type="text"
                name="nameInput"
                required
                minlength="5"
                maxlength="30"
              ></b-form-input>
              <b-input-group-append>
                <b-button variant="primary" type="submit">{{ $t('ADJUST')}}</b-button>
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
</template>

<script lang="ts">
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";
import { mixins } from "vue-class-component";
import { Route } from "vue-router";
import store from "../store";
import { AxiosResponse, AxiosError } from "axios";
import {
  TeamLobbyViewModel,
  ApiResponse,
  SaveTeamMembersResponse
} from "../models/models";
import AccountServiceMixin from "@/services/accountservice";

@Component({
  beforeRouteEnter(to: Route, from: Route, next: any) {
    // called before the route that renders this component is confirmed.
    // does NOT have access to `this` component instance,
    // because it has not been created yet when this guard is called!

    if (!store.state.isLoggedIn) {
      next("/");
    }
    // todo also check the state of the game, you might want to go straight back into the game.
    next();
  }
})
export default class TeamLobby extends mixins(AccountServiceMixin) {
  public name: string = "TeamLobby";

  public inEdit: boolean = false;

  public newName: string = "";

  public teamId: string = "";

  public newMemberNames: string = "";

  public created() {
    this.$store.commit('setNavbarText', 'Team lobby');
    // get team lobby view model
    this.getTeamLobby()
      .then((response: AxiosResponse<TeamLobbyViewModel>) => {
        this.$store.commit("setTeam", response.data.team);
        this.$store.commit("setTeams", response.data.otherTeamsInGame);
        this.teamId = this.$store.state.team.teamId;
        this.newName = this.teamName;
        this.newMemberNames = this.memberNames;
      })
      .catch((error: AxiosError) => {
        this.$bvToast.toast(error.message, {
          solid: true,
          toaster: "b-toaster-bottom-right",
          title: "oops",
          variant: "error"
        });
      });
  }

  public saveMembers(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) return;
    if (this.memberNames === this.newMemberNames) return;

    this.$axios
      .post("api/account/changeteammembers", {
        teamMembers: this.newMemberNames
      })
      .then((response: AxiosResponse<SaveTeamMembersResponse>) => {
        this.$store.commit("setOwnTeamMembers", response.data.teamMembers);
      })
      .catch((error: AxiosError) => {
        this.$bvToast.toast(error.message, {
          title: "oops",
          variant: "error"
        });
      })
      .finally(() => {
        this.newMemberNames = this.memberNames;
      });
  }

  public applyTeamNameChange(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) return;

    // call api that team name changed but only if team name has not changed!
    if (this.teamName === this.newName) return;

    this.$axios
      .post("/api/account/changeteamname", {
        teamId: this.teamId,
        newName: this.newName
      })
      .then((response: AxiosResponse<ApiResponse>) => {
        // only save it to the store if api call is successful!
        this.$store.commit("setOwnTeamName", this.newName);
      })
      .catch((error: AxiosError) => {
        this.$bvToast.toast(error.message, {
          title: "oops",
          variant: "error"
        });
      })
      .finally(() => {
        this.newName = this.teamName;
      });
  }

  @Watch("isLoggedIn") OnLoggedInChanged(value: boolean, oldValue: boolean) {
    if (oldValue && !value) {
      // we've been kicked!
      this.$bvModal
        .msgBoxOk(
          "Jullie zijn verwijderd door de quizmaster. Klik op OK om naar het registratiescherm te gaan.",
          {
            title: "Verwijderd",
            centered: true
          }
        )
        .then(_ => {
          this.$router.push({ name: "RegisterTeam" });
        });
    }
  }

  get isLoggedIn(): boolean {
    return this.$store.state.isLoggedIn || false;
  }

  get teamName() {
    return this.$store.state.team.teamName || "";
  }

  get memberNames() {
    return this.$store.state.team.memberNames || "";
  }

  get teams() {
    return this.$store.state.teams || [];
  }

  get isInEdit() {
    return this.inEdit;
  }
}
</script>
