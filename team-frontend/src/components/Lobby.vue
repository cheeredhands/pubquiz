<template>
<div class="lobby">
    <h1>Welkom in de lobby, {{ team.name }}</h1>
    <hr />
    <p></p>
    <p>Er zijn nu {{ otherTeams.length }} team(s) aan het wachten:</p>
    <ul>
      <li v-for="(team, index) in otherTeams" :key="index">
        {{ item.name }}
      </li>
    </ul>
</div>
</template>
<script>
export default {
  name: "Lobby",
  computed: {
    team() {
      return this.$store.state.quiz.team || "";
    },
    otherTeams() {
      return this.$store.state.quiz.teams;
    }
  }
};
</script>
