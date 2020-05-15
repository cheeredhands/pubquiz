<template>
  <div class="question-container">
    <div class="question-current">
      [Current question]
      <br />
      id: {{quizItem.id}}
      <br/>
      quiz item type: {{quizItem.quizItemType}}
      <br/>
      title: {{quizItem.title}}
      <br/>
      body: {{quizItem.body}}
      <br/>
      interactions: {{quizItem.interactions}}
      <br/>
      media: {{quizItem.media}}
    </div>
    <div class="question-nav">
       <b-button @click="navigateItem(-4)" variant="secondary">
        <font-awesome-icon icon="arrow-left" />
        -4
      </b-button>
      <b-button @click="navigateItem(-1)" variant="secondary">
        <font-awesome-icon icon="arrow-left" />
        {{ $t('PREVIOUS_ITEM') }}
      </b-button>
      {{$t('SECTION')}} {{game.currentSectionIndex}} : {{$t('QUIZ_ITEM')}} {{game.currentQuizItemIndexInSection}} {{$t('OF')}} {{game.currentSectionQuizItemCount}})
      <b-button @click="navigateItem(1)" variant="secondary">
        {{ $t('NEXT_ITEM') }}
        <font-awesome-icon icon="arrow-right" />
      </b-button>
       <b-button @click="navigateItem(4)" variant="secondary">
        +4
        <font-awesome-icon icon="arrow-right" />
      </b-button>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component, { mixins } from 'vue-class-component';
import GameServiceMixin from '../../services/game-service-mixin';
import HelperMixin from '../../services/helper-mixin';
import { AxiosError } from 'axios';
import { ApiResponse } from '../../models/models';

@Component
export default class QmQuestionPart extends mixins(GameServiceMixin, HelperMixin) {
  public name: string = 'qm-question-part';

  // public created() {}

  get game() {
    return this.$store.getters.getGame;
  }

  get quizItem() {
    return this.$store.getters.getQuizItem;
  }

  public navigateItem(offset: number) {
    this.$_gameService_navigateItem(this.game.gameId, offset);
  }
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