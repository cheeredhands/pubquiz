<template>
  <div id="content">
    <h1>Welkom in de Game omgeving, {{userName}}!</h1>
    <b-container fluid>
      <b-col>{{game.gameTitle}} ({{game.state}})</b-col>
    </b-container>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Component } from "vue-property-decorator";
import { Route } from "vue-router";
import store from "../store";
import { AxiosResponse, AxiosError } from "axios";
import { ApiResponse } from "../models/models";

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

export default class QuizMasterGame extends Vue {
  public name: string = "QuizMasterGame";

  public created() {
    // // get QuizMaster Game view model
    // this.$axios
    //   .get("/api/game/quizmastergame")
    //   .then((response: AxiosResponse<QuizMasterGameViewModel>) => {
    //   })
    //   .catch((error: AxiosError) => {
    //     this.$bvToast.toast(error.message, {
    //       title: "oops",
    //       variant: "error"
    //     });
    //   });
  }

  get game() {
    return this.$store.state.game || {};
  }

  get userName() {
    return this.$store.state.user.userName || "";
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
