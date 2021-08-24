<template>
  <div class="main-container">
    <b-modal
      size="xl"
      v-model="isPaused"
      id="modalPaused"
      header-bg-variant="warning"
      centered
      no-close-on-backdrop
      no-close-on-esc
      :title="$t('Paused')"
    >
      <template v-slot:modal-header>
        <h1>{{ $t("Paused") }}</h1>
      </template>
      <template v-slot:default>
        <p class="my-4">{{ $t("THE_GAME_IS_PAUSED") }}</p>
      </template>
      <template v-slot:modal-footer>{{ $t("WAIT_FOR_RESUME") }}</template>
    </b-modal>
    <div class="question-container">
      <b-container fluid>
        <b-row>
          <b-col>
            <h1 :title="`id: ${quizItem.id} type: ${quizItem.quizItemType}`">
              {{ quizItem.title }}
            </h1>
          </b-col>
          <b-col v-if="isReviewing">
            <h2 class="float-right">
              <b-badge variant="success">{{ $t("REVIEWING") }}</b-badge>
            </h2></b-col
          >
        </b-row>
        <b-row>
          <b-col>
            <div><vue-markdown :source="quizItem.body || ''" /></div>
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
                    :class="{
                      correct:
                        isReviewing &&
                        interaction.solution.choiceOptionIds.includes(
                          choiceOption.id
                        ),
                    }"
                    v-for="choiceOption in interaction.choiceOptions"
                    :key="choiceOption.id"
                  >
                    <vue-markdown :source="choiceOption.text || ''" />
                  </li>
                </ul>
              </div>
              <div
                v-else-if="
                  isReviewing && interaction.interactionType === shortAnswer
                "
              >
                <strong>{{ interaction.solution.responses.join(", ") }}</strong>
              </div>
              <div
                v-else-if="
                  isReviewing && interaction.interactionType === extendedText
                "
              >
                <strong>{{ interaction.solution.responses.join(", ") }}</strong>
              </div>
            </div>
          </b-col>
          <b-col v-if="mediaObjects && mediaObjects.length > 0">
            <div v-for="mediaObject in mediaObjects" :key="mediaObject.id">
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
                <vue-markdown :source="mediaObject.text || ''" />
              </div>
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
import { InteractionType, Game, MediaType, QuizItem, GameState, MediaObject } from '../models/models';
import { Watch } from 'vue-property-decorator';
import { QuizItemViewModel } from '../models/viewModels';
import VueMarkdown from 'vue-markdown-render';

@Component({
  components: { VueMarkdown }
})
export default class Beamer extends mixins(GameServiceMixin) {
  public async created(): Promise<void> {
    await this.$_gameService_getQmInGame();
  }

  get game(): Game {
    return (this.$store.state.game || {}) as Game;
  }

  get gameState(): GameState {
    return this.game.state;
  }

  get isPaused(): boolean {
    return this.gameState === GameState.Paused;
  }

  get isReviewing(): boolean {
    return this.gameState === GameState.Reviewing;
  }

  get mediaObjects(): MediaObject[] {
    if (this.quizItem.mediaObjects === undefined) {
      return [];
    }
    if (this.isReviewing) {
      if (this.quizItem.mediaObjects.filter((m) => m.isSolution).length > 0) {
        return this.quizItem.mediaObjects.filter((m) => m.isSolution);
      }
    }
    return this.quizItem.mediaObjects.filter((m) => !m.isSolution);
  }

  get currentQuizItemId(): string {
    return this.$store.getters.currentQuizItemId as string;
  }

  get quizItem(): QuizItem {
    return this.$store.getters.quizItem;
  }

  public name = 'Beamer';
  public multipleChoice: InteractionType = InteractionType.MultipleChoice;
  public multipleResponse: InteractionType = InteractionType.MultipleResponse;
  public shortAnswer: InteractionType = InteractionType.ShortAnswer;
  public extendedText: InteractionType = InteractionType.ExtendedText;
  public imageType: MediaType = MediaType.Image;
  public videoType: MediaType = MediaType.Video;
  public audioType: MediaType = MediaType.Audio;
  public markdownType: MediaType = MediaType.Markdown;

  @Watch('currentQuizItemId') public async OnCurrentItemChanged(value: string): Promise<void> {
    await this.$_gameService_getQuizItem(this.game.id, value);
    // await this.$_gameService_getQuizItemViewModel(this.game.id, value);
    document.title = 'Beamer - ' + this.quizItem.title;
  }
}
</script>

<style scoped>
.question-container {
  padding: 1em;
  height: 100%;
}
body.modal-open .main-container {
  -webkit-filter: blur(12px);
  -moz-filter: blur(12px);
  -o-filter: blur(12px);
  -ms-filter: blur(12px);
  filter: blur(12px);
  filter: progid:DXImageTransform.Microsoft.Blur(PixelRadius='12');
}
li.correct {
  font-weight: bold;
  text-decoration: underline;
  text-decoration-thickness: 3px;
  text-decoration-color: green;
  /* border: 2px solid green;
  border-radius: 10px; */
}
</style>
