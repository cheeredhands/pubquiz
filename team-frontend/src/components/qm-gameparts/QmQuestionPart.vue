<template>
  <div class="question-container">
    <div class="question-current">
      <h1 :title="`id: ${quizItem.id} type: ${quizItem.quizItemType}`">{{quizItem.title}}</h1>
      <p v-html="quizItem.body"></p>
      <div v-for="interaction in quizItem.interactions" :key="interaction.id">
        <p  class="mb-0" :title="interaction.id">{{interaction.text}} ({{interaction.maxScore}} {{$t('POINTS')}})</p>
        <div
          v-if="interaction.interactionType===multipleChoice || interaction.interactionType===multipleResponse"
        >
          <ul>
            <li
              :class="{correct: interaction.solution.choiceOptionIds.includes(choiceOption.id)}"
              v-for="choiceOption in interaction.choiceOptions"
              :key="choiceOption.id"
            >{{choiceOption.text}}</li>
          </ul>
        </div>
        <div v-else-if="interaction.interactionType===shortAnswer">
          <strong>{{interaction.solution.responses.join(', ')}}</strong>
        </div>
        <div v-else-if="interaction.interactionType===extendedText">
          <strong>{{interaction.solution.responses.join(', ')}}</strong>
        </div>
      </div>
      <!-- <p v-if="quizItem.media.length>0">media: {{quizItem.media}}</p> -->
    </div>
    <div class="question-nav">
      <b-button @click="navigateItem(-1)" variant="secondary">
        <font-awesome-icon icon="arrow-left" />
        {{ $t('PREVIOUS_ITEM') }}
      </b-button>
      {{$t('SECTION')}} {{game.currentSectionIndex}} : {{$t('QUIZ_ITEM')}} {{game.currentQuizItemIndexInSection}} {{$t('OF')}} {{game.currentSectionQuizItemCount}})
      <b-button @click="navigateItem(1)" variant="secondary">
        {{ $t('NEXT_ITEM') }}
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
import { InteractionType } from '../../models/models';

@Component
export default class QmQuestionPart extends mixins(
  GameServiceMixin,
  HelperMixin
) {
  public name: string = 'qm-question-part';
  public multipleChoice: InteractionType = InteractionType.MultipleChoice;
  public multipleResponse: InteractionType = InteractionType.MultipleResponse;
  public shortAnswer: InteractionType = InteractionType.ShortAnswer;
  public extendedText: InteractionType = InteractionType.ExtendedText;
  // public created() {}

  get game() {
    return this.$store.getters.game;
  } 

  get quizItem() {
    return this.$store.getters.quizItem;
  }

  public navigateItem(offset: number) {
    this.$_gameService_navigateItem(this.game.id, offset);
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
  padding: 1em;
  border-bottom: 1px solid black;
  background-color: mintcream;
}

.question-nav {
  grid-area: question-nav;
}

li.correct {
  font-weight: bold;
}
</style>