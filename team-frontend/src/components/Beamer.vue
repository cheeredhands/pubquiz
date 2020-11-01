<template>
  <div class="main-container">
    <div class="question-container">
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
            <div
              v-for="interaction in quizItem.interactions"
              :key="interaction.id"
            >
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
                    v-for="choiceOption in interaction.choiceOptions"
                    :key="choiceOption.id"
                  >
                    {{ choiceOption.text }}
                  </li>
                </ul>
              </div>
            </div>
          </b-col>
          <b-col>
            <div
              v-for="mediaObject in quizItem.mediaObjects"
              :key="mediaObject.id"
            >
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
    </div>
  </div>
</template>

<script lang="ts">
import Component, { mixins } from 'vue-class-component';
import GameServiceMixin from '../services/game-service-mixin';
import HelperMixin from '../services/helper-mixin';
import { InteractionType, Game, MediaType, QuizItem } from '../models/models';
import { Watch } from 'vue-property-decorator';
import { debounce } from 'lodash';
import { QuizItemViewModel } from '../models/viewModels';

@Component
export default class Beamer extends mixins(GameServiceMixin) {
  public async created(): Promise<void> {
    await this.$_gameService_getTeamInGame();
    document.title = 'Beamer - ' + this.quizItem.title;
  }

  get game(): Game {
    return (this.$store.state.game || {}) as Game;
  }

  get currentQuizItemId(): string {
    return this.$store.getters.currentQuizItemId as string;
  }

  get quizItem(): QuizItemViewModel {
    return this.$store.getters.quizItemViewModel as QuizItemViewModel;
  }

  public name = 'Beamer';
  public multipleChoice: InteractionType = InteractionType.MultipleChoice;
  public multipleResponse: InteractionType = InteractionType.MultipleResponse;
  public shortAnswer: InteractionType = InteractionType.ShortAnswer;
  public extendedText: InteractionType = InteractionType.ExtendedText;
  public imageType: MediaType = MediaType.Image;
  public videoType: MediaType = MediaType.Video;
  public audioType: MediaType = MediaType.Audio;

  @Watch('currentQuizItemId') public OnCurrentItemChanged(value: string): void {
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
