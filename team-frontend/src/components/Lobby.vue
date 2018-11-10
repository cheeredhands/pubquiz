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
@Component({})
export default class Lobby extends Vue {
  public name: string = 'Lobby';

  public inEdit: boolean = false;

  public newName: string = '';

  public teamId: string = '';

  public created() {
    this.newName = this.team.teamName;
    this.teamId = this.team.teamId;
  }

  public toggleEdit() {
    if (this.inEdit) {
      // call api that team name changed but only if team name has not changed!
      if (this.team.teamName !== this.newName) {
        this.$axios
          .post('/api/account/changeteamname', {
            teamId: this.teamId,
            newName: this.newName,
          })
          .catch((error) => {
            // TODO
          })
          .then(() => {
            // only save it to the store if api call is successful!
            this.$store.commit('setOwnTeamName', this.newName);
          });
      }
    }
    this.inEdit = !this.inEdit;
  }

  get team() {
    return this.$store.state.quiz.team || '';
  }

  get otherTeams() {
    return this.$store.state.quiz.teams;
  }

  get isInEdit() {
    return this.inEdit;
  }
}
</script>
