
<template>
  <div id="app">
    <NavBarPart />
    <b-container fluid>
      <b-row>
        <b-col col cols="1">Buttons</b-col>
        <b-col col cols="5">
          Game title: {{game.gameTitle}} (state: {{game.state}})
          <h2>Teamfeed</h2>
        </b-col>
        <b-col col cols="6">
          Current question and ranking
          <template slot="footerslot">
            <p>Here's some contact info</p>
          </template>
        </b-col>
      </b-row>
    </b-container>
    <FooterPart />
  </div>
</template>


<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Route } from 'vue-router';
import store from '../store';
import { AxiosResponse, AxiosError } from 'axios';
import { ApiResponse } from '../models/models';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';

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
export default class QuizMasterGame extends Vue {
  public name: string = 'QuizMasterGame';

  public created() {
    this.$store.commit('setNavbarText', 'Quiz master game screen');
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
    return this.$store.state.user.userName || '';
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
