<template>
  <b-container fluid>
    <b-row>
      <b-col>
        <h1 :title="`id: ${quizItem.id} type: ${quizItem.quizItemType}`">
          {{ quizItem.title }}
        </h1>
      </b-col>
    </b-row>
    <b-row>
      <b-col>
        <p v-html="quizItem.body"></p>
        <div v-for="interaction in quizItem.interactions" :key="interaction.id">
          <p class="mb-0" :title="interaction.id">
            {{ interaction.text }} ({{ interaction.maxScore }}
            {{ $t("POINTS") }})
          </p>
          <div
            v-if="
              interaction.interactionType === multipleChoice ||
              interaction.interactionType === multipleResponse
            "
          >
            <ul>
              <li
                :class="{
                  correct: interaction.solution.choiceOptionIds.includes(
                    choiceOption.id
                  ),
                }"
                v-for="choiceOption in interaction.choiceOptions"
                :key="choiceOption.id"
              >
                {{ choiceOption.text }}
              </li>
            </ul>
          </div>
          <div v-else-if="interaction.interactionType === shortAnswer">
            <strong>{{ interaction.solution.responses.join(", ") }}</strong>
          </div>
          <div v-else-if="interaction.interactionType === extendedText">
            <strong>{{ interaction.solution.responses.join(", ") }}</strong>
          </div>
        </div>
      </b-col>
      <b-col>
        <div v-for="mediaObject in quizItem.mediaObjects" :key="mediaObject.id">
          <img
            v-if="mediaObject.mediaType === imageType"
            :src="mediaObject.uri"
          />
          <audio
            controls
            v-if="mediaObject.mediaType === audioType"
            :src="mediaObject.uri"
          ></audio>
          <video
            width="320"
            height="240"
            controls
            v-if="mediaObject.mediaType === videoType"
            :src="mediaObject.uri"
          ></video>
        </div>
      </b-col>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import Component, { mixins } from 'vue-class-component';
import GameServiceMixin from '../services/game-service-mixin';
import HelperMixin from '../services/helper-mixin';
import { Game, InteractionType, MediaType, QuizItem } from '../models/models';

@Component
export default class QmQuestionPart extends mixins(
  GameServiceMixin,
  HelperMixin
) {
  public name = 'QuizMasterBeamer';
  public multipleChoice: InteractionType = InteractionType.MultipleChoice;
  public multipleResponse: InteractionType = InteractionType.MultipleResponse;
  public shortAnswer: InteractionType = InteractionType.ShortAnswer;
  public extendedText: InteractionType = InteractionType.ExtendedText;
  public imageType: MediaType = MediaType.Image;
  public videoType: MediaType = MediaType.Video;
  public audioType: MediaType = MediaType.Audio;

  get game(): Game {
    return (this.$store.getters.game || {}) as Game;
  }

  get quizItem(): QuizItem {
    return this.$store.getters.quizItem;
  }

  public navigateItem(offset: number): void {
    this.$_gameService_navigateItem(this.game.id, offset);
  }
}
</script>

<style scoped>

li.correct {
  font-weight: bold;
}
</style>
