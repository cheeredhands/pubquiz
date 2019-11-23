<template>
  <b-container fluid>
    <b-row>
      <b-col>
        <h1>{{ $t('QUIZMASTER_LOGIN') }}</h1>
        <hr />
      </b-col>
    </b-row>
    <b-form @submit="login" novalidate>
      <b-form-row>
        <b-col md="6" lg="4">
          <b-form-group :label="$t('USERNAME')" label-for="userNameInput">
            <b-form-input
              type="text"
              size="lg"
              v-model="userName"
              id="userNameInput"
              name="userNameInput"
              required
              minlength="5"
              maxlength="30"
            />
            <b-form-invalid-feedback>Een gebruikersnaam van minimaal 5 en maximaal 30 karakters is verplicht.</b-form-invalid-feedback>
          </b-form-group>
        </b-col>
      </b-form-row>
      <b-form-row>
        <b-col md="6" lg="4">
          <b-form-group label="Wachtwoord" label-for="passwordInput">
            <b-form-input
              type="password"
              size="lg"
              v-model="password"
              id="passwordInput"
              name="passwordInput"
              required
              minlength="2"
            />
            <b-form-invalid-feedback>Een wachtwoord is minimaal 2 tekens.</b-form-invalid-feedback>
          </b-form-group>
        </b-col>
      </b-form-row>
      <b-form-row>
        <b-col md="6" lg="4">
          <b-button type="submit" variant="primary">Inloggen</b-button>
        </b-col>
      </b-form-row>
    </b-form>
  </b-container>
</template>

<script lang="ts">
import Vue from "vue";
import { AxiosResponse, AxiosError } from "axios";
import { LoginResponse } from "../models/models";
import Component, { mixins } from "vue-class-component";
import AccountServiceMixin from "../services/accountservice";

@Component
export default class QuizMasterLogin extends mixins(AccountServiceMixin) {
  public name: string = "QuizMasterLogin";
  public userName: string = "";
  public password: string = "";

  public mounted() {}

  public login(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) return;
    this.loginUser(this.userName, this.password)
      .then((response: AxiosResponse<LoginResponse>) => {
        this.$store.dispatch("storeToken", response.data.jwt).then(() => {
          this.$store
            .dispatch("initQuizMaster", {
              userId: response.data.userId,
              userName: response.data.userName,
              gameIds: response.data.gameIds,
              isLoggedIn: true
            })
            .then(() => {
              // and goto lobby
              this.$bvToast.toast("Welkom quizmaster!", {
                title: "Welkom!",
                variant: "info"
              });
              this.$router.push({ name: "QuizMasterLobby" });
            });
        });
      })
      .catch((error: AxiosError) => {
        this.$bvToast.toast(error.message, {
          title: "Oops",
          variant: "error"
        });
      });
  }
}
</script>

<style>
</style>
