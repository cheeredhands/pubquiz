<template>
  <div class="question-container">
    <h1 :title="`id: ${quizItem.id} type: ${quizItem.quizItemType}`">{{quizItem.title}}</h1>
    <p v-html="quizItem.body"></p>
    <div v-for="interaction in quizItem.interactions" :key="interaction.id">
      <!-- <p :title="interaction.id">{{interaction.text}} ({{interaction.maxScore}} {{$t('POINTS')}})</p> -->
      <div v-if="interaction.interactionType===multipleChoice">
        <b-form-group :label="interaction.text">
          <b-form-radio
            v-model="interaction.chosenOption"
            v-for="choiceOption in interaction.choiceOptions"
            :key="choiceOption.id"
            :name="`mc${interaction.id}`"
            :value="choiceOption.id"
            @change="submitMcAnswer(interaction.id)"
          >{{choiceOption.text}}</b-form-radio>
        </b-form-group>
      </div>
      <div v-else-if="interaction.interactionType===multipleResponse">
        <b-form-group :label="interaction.text">
          <b-form-checkbox
            v-model="interaction.chosenOptions"
            v-for="choiceOption in interaction.choiceOptions"
            :key="choiceOption.id"
            :name="`mr${interaction.id}`"
            :value="choiceOption.id"
            @change="submitMrAnswer(interaction.id)"
          >{{choiceOption.text}}</b-form-checkbox>
        </b-form-group>
      </div>
      <div v-else-if="interaction.interactionType===shortAnswer">
        <b-form-group :label="`${interaction.text} (${interaction.maxScore})`">
          <b-input-group>
            <b-form-input @keyup="submitTextAnswer(interaction.id)" v-model="interaction.response"></b-form-input>
          </b-input-group>
        </b-form-group>
      </div>
      <div v-else-if="interaction.interactionType===extendedText">
        <b-form-group :label="`${interaction.text} (${interaction.maxScore})`">
          <b-input-group>
            <b-form-textarea
              @keyup="submitTextAnswer(interaction.id)"
              v-model="interaction.response"
            ></b-form-textarea>
          </b-input-group>
        </b-form-group>
      </div>
    </div>
    <!-- <p v-if="quizItem.media.length>0">media: {{quizItem.media}}</p> -->
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component, { mixins } from 'vue-class-component';
import GameServiceMixin from '../../services/game-service-mixin';
import HelperMixin from '../../services/helper-mixin';
import { AxiosError } from 'axios';
import { InteractionType, Game } from '../../models/models';
import { Watch } from 'vue-property-decorator';
import { debounce } from 'lodash';
import { QuizItemViewModel } from '../../models/viewModels';

@Component
export default class TeamQuestionPart extends mixins(
  GameServiceMixin,
  HelperMixin
) {
  get game() {
    return this.$store.getters.game as Game;
  }

  get currentQuizItemId() {
    return this.$store.getters.currentQuizItemId as string;
  }
  get quizItem() {
    return this.$store.getters.quizItemViewModel as QuizItemViewModel;
  }
  public name: string = 'team-question-part';
  public multipleChoice: InteractionType = InteractionType.MultipleChoice;
  public multipleResponse: InteractionType = InteractionType.MultipleResponse;
  public shortAnswer: InteractionType = InteractionType.ShortAnswer;
  public extendedText: InteractionType = InteractionType.ExtendedText;

  public submitTextAnswer = debounce(async (interactionId: number) => {
    await this.$_gameService_submitInteractionResponse(
      this.currentQuizItemId,
      interactionId,
      undefined,
      this.quizItem.interactions[interactionId].response
    );
  }, this.$store.getters.debounceMs);

  public submitMcAnswer = debounce(async (interactionId: number) => {
    // console.log(this.quizItem.interactions[interactionId]);
    const mcAnswer = this.quizItem.interactions[interactionId].chosenOption;
    await this.$_gameService_submitInteractionResponse(
      this.currentQuizItemId,
      interactionId,
      [mcAnswer],
      undefined
    );
  }, this.$store.getters.debounceMs);

  public submitMrAnswer = debounce(async (interactionId: number) => {
    // console.log(this.quizItem.interactions[interactionId]);
    const mcAnswer = this.quizItem.interactions[interactionId].chosenOptions;
    await this.$_gameService_submitInteractionResponse(
      this.currentQuizItemId,
      interactionId,
      mcAnswer,
      undefined
    );
  }, this.$store.getters.debounceMs);

  public navigateItem(offset: number) {
    this.$_gameService_navigateItem(this.game.id, offset);
  }

  @Watch('currentQuizItemId') public OnCurrentItemChanged(
    value: string,
    oldValue: string
  ) {
    this.$_gameService_getQuizItemViewModel(this.game.id, value);
  }
}
</script>

<style scoped>
.question-container {
  padding: 1em;
  height: 100%;
}
</style>