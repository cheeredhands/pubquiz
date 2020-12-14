<template>
  <div class="question-container">
    <div class="question-current">
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
          <b-col
            v-if="quizItem.mediaObjects && mediaObjects.length > 0"
          >
            <div
              v-for="mediaObject in mediaObjects"
              :key="mediaObject.id"
            >
              <figure>
                <b-img
                  fluid
                  rounded
                  v-if="mediaObject.mediaType === imageType"
                  :src="mediaObject.uri"
                />
              </figure>
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
            </div>
          </b-col>
        </b-row>
      </b-container>
    </div>
    <div class="question-nav">
            <b-button @click="navigateItem(-10)" variant="secondary" class="mr-1">
        <font-awesome-icon icon="arrow-left" />
        {{ $t("SKIP_10_BACK") }}
      </b-button>
      <b-button @click="navigateItem(-1)" variant="secondary">
        <font-awesome-icon icon="arrow-left" />
        {{ $t("PREVIOUS_ITEM") }}
      </b-button>
      {{ $t("SECTION") }} {{ game.currentSectionTitle }} ({{ game.currentSectionIndex }}) :
      {{ $t("QUIZ_ITEM") }} {{ game.currentQuizItemIndexInSection }}
      {{ $t("OF") }} {{ game.currentSectionQuizItemCount }})
      <b-button @click="navigateItem(1)" variant="secondary">
        {{ $t("NEXT_ITEM") }}
        <font-awesome-icon icon="arrow-right" />
      </b-button>
      <b-button @click="navigateItem(10)" variant="secondary" class="ml-1">
        {{ $t("SKIP_10_FORWARD") }}
        <font-awesome-icon icon="arrow-right" />
      </b-button>
    </div>
  </div>
</template>

<script lang="ts">
import Component, { mixins } from 'vue-class-component';
import GameServiceMixin from '../../services/game-service-mixin';
import HelperMixin from '../../services/helper-mixin';
import { Game, GameState, InteractionType, MediaObject, MediaType, QuizItem } from '../../models/models';
import { Watch } from 'vue-property-decorator';

@Component
export default class QmQuestionPart extends mixins(
  GameServiceMixin,
  HelperMixin
) {
  public name = 'qm-question-part';
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

  get mediaObjects(): MediaObject[] {
    if (this.game.state === GameState.Reviewing) {
      return this.quizItem.mediaObjects.filter((m) => m.isSolution);
    } else {
      return this.quizItem.mediaObjects.filter((m) => !m.isSolution);
    }
  }

  get currentQuizItemId(): string {
    return this.$store.getters.currentQuizItemId as string;
  }

  public navigateItem(offset: number): void {
    this.$_gameService_navigateItem(this.game.id, offset);
  }

  @Watch('currentQuizItemId') public async OnCurrentItemChanged(value: string): Promise<void> {
    await this.$_gameService_getQuizItem(this.game.id, value);
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
  overflow: hidden;
}

.question-nav {
  padding: 1em;
  grid-area: question-nav;
}

li.correct {
  font-weight: bold;
}
</style>
