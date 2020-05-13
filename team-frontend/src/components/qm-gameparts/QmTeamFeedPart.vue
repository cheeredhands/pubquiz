<template>
  <div class="teamfeed-container">
    <div class="title-bar">
      <h4 class="mt-1 mb-0 ml-1">Team feed (12 teams)</h4>
    </div>
    <div class="feed">
      <ul class="list-unstyled" v-for="i in 12" v-bind:key="i">
        <QmTeamFeedItem />
      </ul>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import QmTeamFeedItem from './QmTeamFeedItem.vue';

@Component({
  components: { QmTeamFeedItem }
})
export default class QmTeamFeedPart extends Vue {
  public name: string = 'QmTeamFeedPart';

  public created() {}
}
</script>

<style scoped>
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