<template>
<div class="lobby">
    <h1>Welkom in de lobby... </h1>
    <hr />
  <p>Er zijn nu {{ nrOfTeams }} team(s) aan het wachten...</p>
</div>
</template>
<script>

export default {
  name: "Lobby",
  props: {
    teams: Array,
  },
  computed: {
    teams() {
      return this.$store.teams;
    }
  }
};
</script>
