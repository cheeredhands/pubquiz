<template>
  <div class="question-container">
    <b-container fluid>
      <b-row>
        <b-col>
          <h1 :title="`id: ${quizItem.id} type: ${quizItem.quizItemType}`">
            {{ quizItem.title }}
          </h1>
        </b-col> </b-row
      ><b-row
        ><b-col>
          <p v-html="quizItem.body"></p>
          <div
            v-for="interaction in quizItem.interactions"
            :key="interaction.id"
          >
            <p :title="interaction.id">{{interaction.text}} ({{interaction.maxScore}}{{$t('POINTS')}})</p>
            <div v-if="interaction.interactionType === multipleChoice">
              <b-form-group>
                <b-form-radio-group
                  v-model="interaction.chosenOption"
                  @change.native="submitMcAnswer(interaction.id)"
                  stacked
                >
                  <b-form-radio
                    v-for="choiceOption in interaction.choiceOptions"
                    :key="choiceOption.id"
                    :name="`mc${interaction.id}`"
                    :value="choiceOption.id"
                    >{{ choiceOption.text }}</b-form-radio
                  >
                </b-form-radio-group>
              </b-form-group>
            </div>
            <div v-else-if="interaction.interactionType === multipleResponse">
              <b-form-group :label="interaction.text">
                <b-form-checkbox
                  v-model="interaction.chosenOptions"
                  v-for="choiceOption in interaction.choiceOptions"
                  :key="choiceOption.id"
                  :name="`mr${interaction.id}`"
                  :value="choiceOption.id"
                  @change.native="submitMrAnswer(interaction.id)"
                  >{{ choiceOption.text }}</b-form-checkbox
                >
              </b-form-group>
            </div>
            <div v-else-if="interaction.interactionType === shortAnswer">
              <b-form-group>
                <b-input-group>
                  <b-form-input
                    @keyup="submitTextAnswer(interaction.id)()"
                    v-model="interaction.response"
                  ></b-form-input>
                </b-input-group>
              </b-form-group>
            </div>
            <div v-else-if="interaction.interactionType === extendedText">
              <b-form-group>
                <b-input-group>
                  <b-form-textarea
                    @keyup="submitTextAnswer(interaction.id)()"
                    v-model="interaction.response"
                  ></b-form-textarea>
                </b-input-group>
              </b-form-group>
            </div>
          </div>
          <p v-if="showChangesSaved" class="text-muted">
            {{ $t("CHANGES_SAVED") }}
          </p>
        </b-col>
        <b-col v-if="quizItem.mediaObjects && quizItem.mediaObjects.length > 0">
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
</template>

<script lang="ts">
import Component, { mixins } from 'vue-class-component';
import GameServiceMixin from '../../services/game-service-mixin';
import HelperMixin from '../../services/helper-mixin';
import { InteractionType, Game, MediaType } from '../../models/models';
import { Watch } from 'vue-property-decorator';
import { throttle, debounce, DebouncedFunc } from 'lodash';
import { QuizItemViewModel } from '../../models/viewModels';
/* eslint space-before-function-paren: "off" */

@Component
export default class TeamQuestionPart extends mixins(
  GameServiceMixin,
  HelperMixin
) {
  get game(): Game {
    return (this.$store.state.game || {}) as Game;
  }

  get currentQuizItemId(): string {
    return this.$store.getters.currentQuizItemId as string;
  }

  get quizItem(): QuizItemViewModel {
    return this.$store.getters.quizItemViewModel as QuizItemViewModel;
  }

  public throttledTextSubmits: DebouncedFunc<() => Promise<void>>[] = [];

  public name = 'team-question-part';
  public multipleChoice: InteractionType = InteractionType.MultipleChoice;
  public multipleResponse: InteractionType = InteractionType.MultipleResponse;
  public shortAnswer: InteractionType = InteractionType.ShortAnswer;
  public extendedText: InteractionType = InteractionType.ExtendedText;
  public imageType: MediaType = MediaType.Image;
  public videoType: MediaType = MediaType.Video;
  public audioType: MediaType = MediaType.Audio;

  public showChangesSaved = true;

  public submitTextAnswer(interactionId: number): DebouncedFunc<() => Promise<void>> {
    if (this.throttledTextSubmits[interactionId] !== undefined) {
      console.log(interactionId);
      return this.throttledTextSubmits[interactionId];
    }
    this.throttledTextSubmits[interactionId] = throttle(async () => {
      console.log(`${interactionId} dus`);
      this.showChangesSaved = false;
      await this.$_gameService_submitInteractionResponse(
        this.currentQuizItemId,
        interactionId,
        undefined,
        this.quizItem.interactions[interactionId].response
      ).then(() => {
        this.showChangesSaved = true;
      });
    }, this.$store.getters.throttleMs);
    return this.throttledTextSubmits[interactionId];
  }

  public submitMcAnswer(interactionId: number): void {
    this.showChangesSaved = false;
    const mcAnswer = this.quizItem.interactions[interactionId].chosenOption;
    this.$_gameService_submitInteractionResponse(
      this.currentQuizItemId,
      interactionId,
      [mcAnswer],
      undefined
    ).then(() => {
      this.showChangesSaved = true;
    });
  }

  public submitMrAnswer(interactionId: number): void {
    const mcAnswer = this.quizItem.interactions[interactionId].chosenOptions;
    this.$_gameService_submitInteractionResponse(
      this.currentQuizItemId,
      interactionId,
      mcAnswer,
      undefined
    );
  }

  public navigateItem(offset: number): void {
    this.$_gameService_navigateItem(this.game.id, offset);
  }

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
