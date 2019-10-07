<template>
  <div id="content">
    <h1>Welkom in de lobby!</h1>
    <b-container fluid>
      <b-row>Sit back and relax..</b-row>
      <hr>
      <b-row>
        <b-col>
          <b-form @submit="applyTeamNameChange" novalidate>
            <b-form-group label="Teamnaam:" description="Houd het netjes!" label-for="nameInput">
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
                  <b-button variant="primary" type="submit">Aanpassen</b-button>
                </b-input-group-append>
                <b-form-invalid-feedback>Een teamnaam van minimaal 5 en maximaal 30 karakters is verplicht.</b-form-invalid-feedback>
              </b-input-group>
            </b-form-group>
          </b-form>
          <b-form @submit="saveMembers" novalidate>
            <b-form-group
              label="Teamleden:"
              label-for="memberNamesInput"
              description="Geef hier de namen van je teamleden op (een per regel)."
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
                  <b-button variant="primary" type="submit">Opslaan</b-button>
                </b-input-group-append>
              </b-input-group>
            </b-form-group>
          </b-form>
        </b-col>
        <b-col>
          <p>Jullie gaan het opnemen tegen:</p>
          <b-list-group>
            <b-list-group-item
              v-for="(otherTeam, index) in teams"
              :key="index"
              :title="otherTeam.memberNames"
            >
              {{ otherTeam.teamName }}
              <span v-if="!otherTeam.isLoggedIn">(uitgelogd)</span>
            </b-list-group-item>
          </b-list-group>
        </b-col>
      </b-row>
      <!-- <input v-if="isInEdit" v-model="newName" id="teamName" />
    <div v-else>
      {{ team.teamName }}
      </div>-->
    </b-container>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Component} from "vue-property-decorator";
import { mixins } from "vue-class-component";
import { Route } from "vue-router";
import store from "../store";
import { AxiosResponse, AxiosError } from "axios";
import {
  TeamLobbyViewModel,
  ApiResponse,
  SaveTeamMembersResponse
} from "../models/models";
import AccountServiceMixin from '@/services/accountservice';

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
        this.$snotify.error(error.message);
      });
  }

  public saveMembers(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) return;
    if (this.memberNames === this.newMemberNames) return;

    this.$axios
      .post("api/account/changeteammembers", {
        teamId: this.teamId,
        teamMembers: this.newMemberNames
      })
      .then((response: AxiosResponse<SaveTeamMembersResponse>) => {
        this.$store.commit("setOwnTeamMembers", response.data.teamMembers);
        this.$snotify.success(response.data.message);
      })
      .catch((error: AxiosError) => {
        this.$snotify.error(error.message);
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
        this.$snotify.success(response.data.message);
      })
      .catch((error: AxiosError) => {
        this.$snotify.error(error.message);
      })
      .finally(() => {
        this.newName = this.teamName;
      });
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
