<template>
  <div class="question-container">
    <div class="question-current">[Current question]</div>
    <div class="question-nav">[Question navigation]</div>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';

@Component
export default class QmQuestionPart extends Vue {
  public name: string = 'qm-question-part';
}
</script>

<style scoped>
.question-container {
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 6fr 1fr;
  grid-template-areas: "question-current" "question-nav";
  height: 100%;
}

.question-current {
  grid-area: question-current;
  border-bottom: 1px solid black;
}

.question-nav {
  grid-area: question-nav;
}
</style>