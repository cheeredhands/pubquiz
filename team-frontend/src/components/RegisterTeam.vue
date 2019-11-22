<template>
  <div id="content">
    <b-container>
      <b-row>
        <h1>{{$t('REGISTER')}}</h1>
      </b-row>
      <hr>
      <b-row>
        <b-form @submit="register" novalidate>
          <b-form-group
            :label="$t('TEAMNAME')"
            :description="$t('KEEP_IT_CLEAN')"
            label-for="teamNameInput">
            <b-form-input
              type="text"
              size="lg"
              v-model="teamName"
              id="teamNameInput"
              name="teamNameInput"
              required
              minlength="5"
              maxlength="30"
            />
            <b-form-invalid-feedback>{{ $t('TEAMNAME_LENGTH') }}</b-form-invalid-feedback>
          </b-form-group>
          <b-form-group
            :label="$t('CODE')"
            label-for="codeInput"
            :description="$t('CODE_ORIGIN')"
          >
            <b-form-input
              type="text"
              size="lg"
              v-model="code"
              id="codeInput"
              name="codeInput"
              required
              minlength="4"
            />
            <b-form-invalid-feedback>{{ $t('CODE_LENGTH') }}</b-form-invalid-feedback>
          </b-form-group>
          <b-button type="submit" variant="primary">{{ $t('REGISTER') }}</b-button>
        </b-form>
      </b-row>
    </b-container>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { mixins } from "vue-class-component";
import AccountServiceMixin from "../services/accountservice";
import { AxiosResponse, AxiosError } from "axios";
import VueI18n from "vue-i18n";
import { TeamInfo, RegisterForGameResponse } from "../models/models";
import { mixins } from "vue-class-component";
import AccountServiceMixin from "../services/accountservice";

@Component
export default class RegisterTeam extends mixins(AccountServiceMixin) {
  public name: string = "RegisterTeam";
  public teamName: string = "";
  public code: string = "";

  public mounted() {}

  public register(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) return;

    // register!
    this.registerForGame(this.teamName, this.code)
      .then((response: AxiosResponse<RegisterForGameResponse>) => {
        this.$store.dispatch("storeToken", response.data.jwt).then(() => {
          // disco. init team (add team to store, start signalr)
          this.$store
            .dispatch("initTeam", {
              teamId: response.data.teamId,
              teamName: response.data.teamName,
              memberNames: response.data.memberNames,
              isLoggedIn: true
            })
            .then(() => {
              this.$router.push({ name: "TeamLobby" });
            });
        });
      })
      .catch((error: AxiosError) => {
        this.$bvToast.toast(error.message, {
          title: "oops",
          variant: "error"
        });
      });
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
