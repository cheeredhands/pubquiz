<template>
  <div class="teamfeed-container">
    <div class="title-bar">
      <h4 class="mt-1 mb-0 ml-1">Team feed ({{ qmTeams.length }} teams)</h4>
    </div>
    <div class="feed">
      <b-list-group flush>
        <b-list-group-item
          class="w-100"
          v-for="team in qmTeams"
          :key="team.id"
          :variant="getListVariant(team)"
        >
          <!-- <template v-slot:aside>
            <b-img
              blank
              blank-color="#abc"
              width="64"
              alt="placeholder"
            ></b-img>
          </template> -->
          <!-- <p class="mb-0 text-right" style="font-size: 0.6em; width: 30em; display:none">
            The team avatar to the left has a status badge overlay.
            This area shows the answers a team gives to the current question (as they are typing).
            The score and correctness of a team is shown. When automatic scoring is not possible, buttons are shown to mark the answer.
          </p>-->
          <h5
            class="mt-0 mb-1 d-inline-block"
            v-b-tooltip.click.right
            :title="team.recoveryCode"
          >
            {{ team.name
            }}<b-icon-exclamation-triangle
              class="ml-1"
              :title="$t('TEAM_OFFLINE')"
              v-if="!team.isLoggedIn"
            ></b-icon-exclamation-triangle>
          </h5>
          <b-badge
            pill
            :title="$t('NUMBER_OF_CONNECTIONS')"
            v-b-tooltip.hover
            variant="primary"
            class="ml-1 mb-1"
            v-if="team.connectionCount > 1"
            >{{ team.connectionCount }}</b-badge
          >
          <small
            v-if="team.memberNames !== undefined && team.memberNames !== ''"
            class="d-inline-block align-bottom mb-1 ml-2 w-75 text-truncate"
          >
            {{ team.memberNames }}
          </small>

          <div v-if="team.answers[game.currentQuizItemId] !== undefined">
            <p
              v-for="interactionResponse in team.answers[game.currentQuizItemId]
                .interactionResponses"
              :key="interactionResponse.id"
              class="mb-0"
            >
              <b-icon-eyeglasses
                class="float-right mr-1 mt-1"
                :title="$t('FLAGGED')"
                v-if="interactionResponse.flaggedForManualCorrection"
              />

              {{ getInteraction(interactionResponse.interactionId).text }}:
              <code
                class="rounded p-1"
                :class="getResponseBackgroundClass(interactionResponse)"
                >{{ getResponseText(interactionResponse) }}</code
              >
              <b-badge
                href="#"
                pill
                variant="success"
                v-b-tooltip
                :title="$t('SET_OUTCOME_CORRECT')"
                class="ml-1 mb-1"
                @click="
                  correctInteraction(
                    team.id,
                    quizItem.id,
                    interactionResponse.interactionId,
                    true
                  )
                "
              >
                <b-icon-check-circle-fill/></b-badge>
              <b-badge
                href="#"
                pill
                variant="danger"
                v-b-tooltip
                :title="$t('SET_OUTCOME_INCORRECT')"
                class="ml-1 mb-1"
                @click="
                  correctInteraction(
                    team.id,
                    quizItem.id,
                    interactionResponse.interactionId,
                    false
                  )
                "
              >
                <b-icon-x-circle-fill /></b-badge>
            </p>
          </div>
        </b-list-group-item>
      </b-list-group>
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
    return this.$store.getters.qmTeams as Team[];
  }

  public getListVariant(team: Team): string {
    // if (team === undefined) return '';
    if (!team.isLoggedIn) return 'light';
    if (Object.keys(team.answers).length > 0) {
      const answer = team.answers[this.game.currentQuizItemId];
      if (answer === undefined) return '';
      if (answer.flaggedForManualCorrection) return 'warning';
      return (answer.totalScore === this.quizItem.maxScore) ? 'success' : 'secondary';
    }
    return '';
  }

  public getResponseBackgroundClass(interactionResponse: InteractionResponse): string {
    if (interactionResponse.awardedScore === 0) return 'incorrect';
    if (interactionResponse.flaggedForManualCorrection) return 'flagged';
    const interaction = this.quizItem.interactions.find(i => i.id === interactionResponse.interactionId);
    if (interaction?.maxScore === interactionResponse.awardedScore) return 'correct';
    return '';
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
span.smaller {
  font-size: small;
  margin-bottom: 0;
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
  /* border-bottom: 1px solid black; */
}

.feed {
  grid-area: feed;
  height: 100%;
  overflow: auto;
  /* padding: 5px; */
}

.correct {
  background-color: lightgreen;
}
.incorrect {
  background-color: lightpink;
}
.flagged {
  background-color: lightsalmon;
}

code {
  color: black;
}
</style>
