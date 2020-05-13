<template>
  <b-media tag="li">
    <template v-slot:aside>
      <b-img blank blank-color="#abc" width="64" alt="placeholder"></b-img>
    </template>
    <h5 class="mt-0 mb-1">Team 1 answered</h5>
    <p class="mb-0">
      Cras sit amet nibh libero, in gravida nulla. Nulla vel metus scelerisque ante sollicitudin.
      Cras purus odio, vestibulum in vulputate at, tempus viverra turpis.
    </p>
  </b-media>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';

@Component
export default class QmTeamFeedItem extends Vue {
  public name: string = 'QmTeamFeedItem';
}
</script>

<style scoped>

h5 {
    font-size: 1em;
}
p {
    font-size: smaller;
}

</style>