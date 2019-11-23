<template>
  <b-container fluid>
    <b-row>
      <b-col>
       Game title: {{game.gameTitle}} (state: {{game.state}})
        <b-button
          v-on:disabled="game.state==GameState.Running"
          v-on:click="startGame"
          variant="success"
        >Start Game</b-button>
        <hr />
      </b-col>
    </b-row>
    <b-row>
      <b-col lg="6">
        <p>De volgende teams zitten in de teamlobby:</p>
        <b-list-group>
          <b-list-group-item v-for="(team, index) in teams" :key="index">
            <strong>{{ team.teamName }}</strong> -
            <span class="teamMembers">{{team.memberNames}}</span>&nbsp;
            <b-badge v-if="!team.isLoggedIn">uitgelogd</b-badge>
            <font-awesome-icon
              icon="trash-alt"
              @click="kickTeam(team.teamId)"
              pull="right"
              style="cursor:pointer;"
              title="Kick this team from the game"
            />
          </b-list-group-item>
        </b-list-group>
      </b-col>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import Vue from "vue";
import { Component } from "vue-property-decorator";
import { Route } from "vue-router";
import store from "../store";
import { AxiosResponse, AxiosError } from "axios";
import { mixins } from "vue-class-component";
import AccountServiceMixin from "../services/accountservice";
import {
  QuizMasterLobbyViewModel,
  ApiResponse,
  GameState,
  GameStateChanged
} from "../models/models";

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
export default class QuizMasterLobby extends mixins(AccountServiceMixin) {
  public name: string = "QuizMasterLobby";

  public created() {
    this.$store.commit('setNavbarText', 'Quiz master lobby');
    // get team lobby view model
    this.$axios
      .get("/api/game/quizmasterlobby")
      .then((response: AxiosResponse<QuizMasterLobbyViewModel>) => {
        this.$store.commit("setTeams", response.data.teamsInGame);
        this.$store.commit("setGame", response.data.currentGame);
      })
      .catch((error: AxiosError) => {
        this.$bvToast.toast(error.message, {
          title: "oops",
          variant: "error"
        });
      });
  }

  startGame() {
    this.setGameState(this.userId, this.game.gameId, GameState.Running)
      .then(() => {
        //go to gameComponent
        this.$router.push({ name: "QuizMasterGame" });
      })
      .catch((error: AxiosError) => {
        this.$bvToast.toast(error.message, {
          title: "oops",
          variant: "error"
        });
      });
  }

  kickTeam(teamId: string) {
    this.deleteTeam(teamId)
      .then(() => {
        this.$bvToast.toast("Team removed from the game.", {
          title: "Team removed",
          variant: "warning"
        });
      })
      .catch((error: AxiosError) => {
        this.$bvToast.toast(error.message, {
          title: "oops",
          variant: "error"
        });
      });
  }

  messageTeam() {
    this.$bvToast.toast("todo: send message to team", {
      title: "todo",
      variant: "warning"
    });
  }

  get teams() {
    return this.$store.state.teams || [];
  }

  get game() {
    return this.$store.state.game || {};
  }

  get userName() {
    return this.$store.state.user.userName || "";
  }

  get userId() {
    return this.$store.state.userId || "";
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.teamMembers {
  font-size: 0.8em;
}
</style>
