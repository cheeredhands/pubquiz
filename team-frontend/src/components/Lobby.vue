<template>
<div class="lobby">
    <h1>Welkom in de lobby!</h1>
    <span>Sit back and relax...</span>
    <hr />
    <input v-if="isInEdit" v-model="newName" id="teamName" />
    <div v-else>
      {{ team.teamName }}
    </div>
    <button @click="toggleEdit()">
      <span v-if="isInEdit">OK</span>
      <span v-else>Wijzig naam</span>
    </button>

   <hr />
    <p>Jullie gaan het opnemen tegen:</p>
    <ul>
      <li v-for="(team, index) in otherTeams" :key="index">
        {{ team.teamName }}
      </li>
    </ul>
</div>
</template>
<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Route } from 'vue-router';
import store from '../store';
import { AxiosResponse, AxiosError } from 'axios';
import { TeamLobbyViewModel } from '../models/models';

@Component({
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
export default class Lobby extends Vue {
  public name: string = 'Lobby';

  public inEdit: boolean = false;

  public newName: string = '';

  public teamId: string = '';

   public created() {
    this.newName = this.team.teamName;
    this.teamId = this.team.teamId;

    // get team lobby view model
    this.$axios
      .get('/api/game/teamlobby')
      .then((response: AxiosResponse<TeamLobbyViewModel>)=>{
        this.$store.commit('setTeam',response.data.team);
        this.$store.commit('setOtherTeams', response.data.otherTeamsInGame);
      })
      .catch((error: AxiosError)=>{
        // todo
      });
  }

  public toggleEdit() {
    if (this.inEdit) {
      // call api that team name changed but only if team name has not changed!
      if (this.team.teamName !== this.newName) {
        this.$axios
          .post('/api/account/changeteamname', {
            teamId: this.teamId,
            newName: this.newName
          })         
          .then(() => {
            // only save it to the store if api call is successful!
            this.$store.commit('setOwnTeamName', this.newName);
          })
           .catch((error: AxiosError) => {
            // TODO
          });
      }
    }
    this.inEdit = !this.inEdit;
  }

  get team() {
    return this.$store.state.team || '';
  }

  get otherTeams() {
    return this.$store.state.otherTeams;
  }

  get isInEdit() {
    return this.inEdit;
  }
}
</script>
