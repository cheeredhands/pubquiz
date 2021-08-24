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
          <div><vue-markdown :source="quizItem.body || ''" /></div>
          <div
            v-for="interaction in quizItem.interactions"
            :key="interaction.id"
          >
            <p :title="interaction.id">
              {{ interaction.text }} ({{ interaction.maxScore
              }}{{ $t("POINTS") }})
            </p>
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
                    ><vue-markdown :source="choiceOption.text || ''"
                  /></b-form-radio>
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
          <small
            v-if="showChangesSaved && quizItem.interactions.length > 0"
            class="text-muted fade-in"
          >
            {{ $t("CHANGES_SAVED") }}
          </small>
        </b-col>
        <b-col v-if="quizItem.mediaObjects && quizItem.mediaObjects.length > 0">
          <div
            v-for="mediaObject in quizItem.mediaObjects.filter(
              (m) => m.teamVisible
            )"
            :key="mediaObject.id"
          >
            <b-img
              fluid
              rounded
              v-if="mediaObject.mediaType === imageType"
              :src="mediaObject.uri"
            />
            <audio
              controls
              v-if="mediaObject.mediaType === audioType"
              :src="mediaObject.uri"
            ></audio>
            <b-embed
              v-if="mediaObject.mediaType === videoType"
              type="video"
              controls
              :src="mediaObject.uri"
            ></b-embed>
            <div v-if="mediaObject.mediaType === markdownType">
              <vue-markdown :source="mediaObject.text" />
            </div>
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
import VueMarkdown from 'vue-markdown-render';
/* eslint space-before-function-paren: "off" */

@Component({
  components: { VueMarkdown }
})
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

  public debouncedTextSubmits: DebouncedFunc<() => Promise<void>>[] = [];

  public name = 'team-question-part';
  public multipleChoice: InteractionType = InteractionType.MultipleChoice;
  public multipleResponse: InteractionType = InteractionType.MultipleResponse;
  public shortAnswer: InteractionType = InteractionType.ShortAnswer;
  public extendedText: InteractionType = InteractionType.ExtendedText;
  public imageType: MediaType = MediaType.Image;
  public videoType: MediaType = MediaType.Video;
  public audioType: MediaType = MediaType.Audio;
  public markdownType: MediaType = MediaType.Markdown;

  public showChangesSaved = false;

  public submitTextAnswer(interactionId: number): DebouncedFunc<() => Promise<void>> {
    this.showChangesSaved = false;
    if (this.debouncedTextSubmits[interactionId] !== undefined) {
      return this.debouncedTextSubmits[interactionId];
    }
    this.debouncedTextSubmits[interactionId] = debounce(async () => {
      await this.$_gameService_submitInteractionResponse(
        this.currentQuizItemId,
        interactionId,
        undefined,
        this.quizItem.interactions[interactionId].response
      ).then(() => {
        this.showChangesSaved = true;
      });
    }, this.$store.getters.throttleMs);
    return this.debouncedTextSubmits[interactionId];
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

.fade-in {
  animation: fadeIn ease 1s;
  -webkit-animation: fadeIn ease 1s;
  -moz-animation: fadeIn ease 1s;
  -o-animation: fadeIn ease 1s;
  -ms-animation: fadeIn ease 1s;
}
@keyframes fadeIn {
  0% {
    opacity: 0;
  }
  100% {
    opacity: 1;
  }
}

@-moz-keyframes fadeIn {
  0% {
    opacity: 0;
  }
  100% {
    opacity: 1;
  }
}

@-webkit-keyframes fadeIn {
  0% {
    opacity: 0;
  }
  100% {
    opacity: 1;
  }
}

@-o-keyframes fadeIn {
  0% {
    opacity: 0;
  }
  100% {
    opacity: 1;
  }
}

@-ms-keyframes fadeIn {
  0% {
    opacity: 0;
  }
  100% {
    opacity: 1;
  }
}
</style>
