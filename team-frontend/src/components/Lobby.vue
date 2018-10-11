<template>
<div class="lobby">
    <h1>Welkom in de lobby... </h1>
    <hr />
  <p>Er zijn nu {{ otherTeams }} team(s) aan het wachten...</p>
</div>
</template>
<script>

export default {
  name: "Lobby",
  props: {
    teams: Array,
  },
  computed: {
    otherTeams() {
      return this.$store.teams;
    }
  }
};
</script>
