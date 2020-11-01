<template>
  <div class="teamfeed-container">
    <div class="title-bar">
      <h4 class="mt-1 mb-0 ml-1">Team feed ({{ qmTeams.length }} teams)</h4>
    </div>
    <div class="feed">
      <!-- <p>quizItemId: {{game.currentQuizItemId}}</p> -->
      <!-- <p>my team: {{qmTeams.find(t=>t.name==='saxcasdf')}}</p> -->
      <ul class="list-unstyled">
        <b-media class="mb-2" tag="li" v-for="team in qmTeams" :key="team.id">
          <template v-slot:aside>
            <b-img
              blank
              blank-color="#abc"
              width="64"
              alt="placeholder"
            ></b-img>
          </template>
          <!-- <p class="mb-0 text-right" style="font-size: 0.6em; width: 30em; display:none">
            The team avatar to the left has a status badge overlay.
            This area shows the answers a team gives to the current question (as they are typing).
            The score and correctness of a team is shown. When automatic scoring is not possible, buttons are shown to mark the answer.
          </p>-->
          <h5 class="mt-0 mb-1">
            {{ team.name }}
            <span v-if="team.memberNames !== undefined" class="smaller"
              >({{ team.memberNames }})</span
            >
          </h5>
          <div
            v-if="team.answers[game.currentQuizItemId] !== undefined"
            :class="{
              correct:
                team.answers[game.currentQuizItemId].totalScore ===
                quizItem.maxScore,
            }"
          >
            <font-awesome-icon
              icon="glasses"
              class="float-right mr-3"
              title="Flagged for manual correction"
              v-if="
                team.answers[game.currentQuizItemId].flaggedForManualCorrection
              "
            />
            <p
              v-for="interactionResponse in team.answers[game.currentQuizItemId]
                .interactionResponses"
              :key="interactionResponse.id"
              :class="{ correct: interactionResponse.awardedScore > 0 }"
            >
             <font-awesome-icon
              :title="$t('SET_OUTCOME_CORRECT')"
              class="mr-1"
              icon="check-circle"
              @click="
                correctInteraction(
                  team.id,
                  quizItem.id,
                  interactionResponse.interactionId,
                  true
                )
              "
            />
            <font-awesome-icon
              :title="$t('SET_OUTCOME_INCORRECT')"
              class="mr-1"
              icon="times-circle"
              @click="
                correctInteraction(
                  team.id,
                  quizItem.id,
                  interactionResponse.interactionId,
                  false
                )
              "
            />
              {{ getInteraction(interactionResponse.interactionId).text }}:
              <code>{{ getResponseText(interactionResponse) }}</code>
            </p>
          </div>
        </b-media>
      </ul>
    </div>
  </div>
</template>

<script lang="ts">
import GameServiceMixin from '@/services/game-service-mixin';
import Vue from 'vue';
import Component, { mixins } from 'vue-class-component';
import { Game, Interaction, InteractionType, QuizItem, InteractionResponse, Team } from '../../models/models';

@Component
export default class QmTeamFeedPart extends mixins(GameServiceMixin) {
  public name = 'QmTeamFeedPart';

  get game(): Game {
    return (this.$store.getters.game || {}) as Game;
  }

  get quizItem(): QuizItem {
    return this.$store.getters.quizItem as QuizItem;
  }

  get qmTeams(): Team[] {
    return (this.$store.getters.qmTeams || []) as Team[];
  }

  public getInteraction(interactionId: number): Interaction {
    return this.quizItem.interactions[interactionId];
  }

  public getResponseText(interactionResponse: InteractionResponse): string {
    const interaction = this.getInteraction(interactionResponse.interactionId);
    if (
      interaction.interactionType === InteractionType.MultipleChoice ||
      interaction.interactionType === InteractionType.MultipleResponse
    ) {
      return interactionResponse.choiceOptionIds
        .map(i => interaction.choiceOptions[i].text)
        .join(',');
    } else {
      return interactionResponse.response;
    }
  }

  public async correctInteraction(teamId: string, quizItemId: string, interactionId: string, correct: boolean): Promise<void> {
    await this.$_gameService_correctInteraction(teamId, quizItemId, interactionId, correct);
  }
}
</script>

<style scoped>
span.smaller,
p {
  font-size: small;
}

.teamfeed-container {
  display: grid;
  /* grid-template-columns: 1fr; */
  grid-template-rows: 40px 1fr;
  grid-template-areas: "title-bar" "feed";
  height: 100%;
  overflow: hidden;
}

.title-bar {
  grid-area: title-bar;
  border-bottom: 1px solid black;
}

.feed {
  grid-area: feed;
  height: 100%;
  overflow: auto;
  padding: 5px;
}

p.correct {
  background-color: lightgreen;
}

code {
  color: black;
}
</style>
