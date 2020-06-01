<template>
  <div class="teamfeed-container">
    <div class="title-bar">
      <h4 class="mt-1 mb-0 ml-1">Team feed (12 teams)</h4>
    </div>
    <div class="feed">
      <ul class="list-unstyled">
        <b-media tag="li" v-for="i in 12" v-bind:key="i">
          <template v-slot:aside>
            <b-img blank blank-color="#abc" width="64" alt="placeholder"></b-img>
          </template>
          <h5 class="mt-0 mb-1">Team {{i}}</h5>
          <p class="mb-0">
            The team avatar to the left has a status badge overlay.
            This area shows the answers a team gives to the current question (as they are typing).
            The score and correctness of a team is shown. When automatic scoring is not possible, buttons are shown to mark the answer.
          </p>
        </b-media>
      </ul>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';

@Component
export default class TeamChatPart extends Vue {
  public name = 'TeamChatPart';

  // public created() {}
}
</script>

<style scoped>
h5 {
  font-size: 1em;
}

p {
  font-size: smaller;
}

.teamfeed-container {
  display: grid;
  /* grid-template-columns: 1fr; */
  grid-template-rows: 40px 1fr;
  grid-template-areas: "title-bar" "feed";
  height: 100%;
  overflow: hidden;
}

.title-bar {
  grid-area: title-bar;
  border-bottom: 1px solid black;
}

.feed {
  grid-area: feed;
  height: 100%;
  overflow: auto;
}
</style>
