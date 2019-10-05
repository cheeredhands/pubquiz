<template>
  <div id="content">
    <h1>Welkom in de lobby, {{userName}}!</h1>
    <b-container fluid>
      <b-row>&lt;ondertitel&gt;</b-row>
      <hr />
      <b-row>
        <b-col>
          <p>De volgende teams zitten in de teamlobby:</p>
          <b-list-group>
            <b-list-group-item
              :disabled="!team.isLoggedIn"
              v-for="(team, index) in teams"
              :key="index"
            >
              <strong>{{ team.teamName }}</strong> -
              <span class="teamMembers">{{team.memberNames}}</span>&nbsp;
              <b-badge v-if="!team.isLoggedIn">uitgelogd</b-badge>
              <span v-if="team.isLoggedIn">
                <font-awesome-icon
                  icon="sign-out-alt"
                  @click="kickTeam"
                  pull="right"
                  style="cursor:pointer;"
                  title="Kick this team from the game"
                />
                <font-awesome-icon
                  icon="comment"
                  @click="messageTeam"
                  pull="right"
                  style="cursor:pointer;"
                  title="Send this team a message"
                />
              </span>
            </b-list-group-item>
          </b-list-group>
        </b-col>
        <b-col></b-col>
      </b-row>
    </b-container>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Component } from "vue-property-decorator";
import { Route } from "vue-router";
import store from "../store";
import { AxiosResponse, AxiosError } from "axios";
import { QuizMasterLobbyViewModel, ApiResponse } from "../models/models";

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
export default class QuizMasterLobby extends Vue {
  public name: string = "QuizMasterLobby";

  public created() {
    // get team lobby view model
    this.$axios
      .get("/api/game/quizmasterlobby")
      .then((response: AxiosResponse<QuizMasterLobbyViewModel>) => {
        this.$store.commit("setTeams", response.data.teamsInGame);
      })
      .catch((error: AxiosError) => {
        this.$snotify.warning(error.message);
      });
  }

  kickTeam() {
    this.$bvToast.toast("todo: implement kick team", {
      solid: true,
      toaster: "b-toaster-bottom-right",
      title: "todo",
      variant: "warning"
    });
  }

  messageTeam() {
    this.$bvToast.toast("todo: send message to team", {
      solid: true,
      toaster: "b-toaster-bottom-right",
      title: "todo",
      variant: "warning"
    });
  }

  get teams() {
    return this.$store.state.teams || [];
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