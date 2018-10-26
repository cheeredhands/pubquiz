<template>
<div class="lobby">
    <h1>Welkom in de lobby!</h1>
    <span>Sit back and relax...</span>
    <hr />
     <div v-if="isInEdit">
        <input v-model="newName" id="teamName" />
     </div>
     <div v-else>
        {{ team.name }}
     </div>
      <button @click="toggleEdit()">
        <span v-if="isInEdit">OK</span>
        <span v-else>Wijzig naam</span>
      </button>

   <hr />
    <p>Jullie gaan het opnemen tegen:</p>
    <ul>
      <li v-for="(team, index) in otherTeams" :key="index">
        {{ team.name }}
      </li>
    </ul>
</div>
</template>
<script>
export default {
  name: "Lobby",
  data() {
    return {
      inEdit: false,
      newName: "",
      teamId: ""
    }
  },
  mounted() {
      this.newName = this.team.teamName,
      this.teamId = this.team.teamId
  },
  methods: {
    toggleEdit() {
      if (this.inEdit) {
        // call api that team name changed.
        this.$axios.post('/api/account/changeteamname', {
          teamId: this.teamId,
          newName: this.newName
        })
        .then(response => {
          this.$store.commit("setOwnTeamName", {
            newName: this.newName
          });
        })
      }

      this.inEdit = !this.inEdit;
    }
  },
  computed: {
    team() {
      return this.$store.state.quiz.team || "";
    },
    otherTeams() {
      return this.$store.state.quiz.teams;
    },
    isInEdit() {
      return this.inEdit;
    }
  }
};
</script>
