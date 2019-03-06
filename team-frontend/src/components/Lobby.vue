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
                  v-model="memberNames"
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
              v-for="(team, index) in otherTeams"
              :key="index"
              :disabled="!team.isLoggedIn"
            >
              {{ team.teamName }}
              <span v-if="!team.isLoggedIn">(uitgelogd)</span>
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
import { Component } from "vue-property-decorator";
import { Route } from "vue-router";
import store from "../store";
import { AxiosResponse, AxiosError } from "axios";
import { TeamLobbyViewModel, ApiResponse } from "../models/models";

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
export default class Lobby extends Vue {
  public name: string = "Lobby";

  public inEdit: boolean = false;

  public newName: string = "";

  public teamId: string = "";

  public memberNames: string = "";

  public created() {
    this.newName = this.team.teamName;
    this.teamId = this.team.teamId;
    this.memberNames = this.team.memberNames;

    // get team lobby view model
    this.$axios
      .get("/api/game/teamlobby")
      .then((response: AxiosResponse<TeamLobbyViewModel>) => {
        this.$store.commit("setTeam", response.data.team);
        this.$store.commit("setOtherTeams", response.data.otherTeamsInGame);
      })
      .catch((error: AxiosError) => {
        this.$snotify.error(error.message);
      });
  }

  public saveMembers(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) return;
    if (this.team.memberNames === this.memberNames) return;

    this.$axios
      .post("api/account/changeteammembers", {
        teamId: this.team.teamId,
        teamMembers: this.memberNames
      })
      .then((response: AxiosResponse<ApiResponse>) => {
        this.$store.commit("setOwnTeamMembers", this.memberNames);
        this.$snotify.success(response.data.message);
      })
      .catch((error: AxiosError) => {
        this.$snotify.error(error.message);
      });
  }

  public applyTeamNameChange(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) return;

    // call api that team name changed but only if team name has not changed!
    if (this.team.teamName === this.newName) return;

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
      });
  }

  get team() {
    return this.$store.state.team || "";
  }

  get otherTeams() {
    return this.$store.state.otherTeams;
  }

  get isInEdit() {
    return this.inEdit;
  }
}
</script>
