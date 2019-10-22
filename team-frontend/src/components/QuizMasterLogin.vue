<template>
  <div id="content">
    <b-container>
      <b-row>
        <h1>Quizmaster / admin login</h1>
      </b-row>
      <hr />
      <b-row>
        <b-form @submit="login" novalidate>
          <b-form-group label="Gebruikersnaam:" label-for="userNameInput">
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
          <b-form-group label="Wachtwoord:" label-for="passwordInput">
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
          <b-button type="submit" variant="primary">Inloggen</b-button>
        </b-form>
      </b-row>
    </b-container>
    <div></div>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { AxiosResponse, AxiosError } from "axios";
import { LoginResponse } from "../models/models";
import Component from "vue-class-component";

@Component
export default class QuizMasterLogin extends Vue {
  public name: string = "QuizMasterLogin";
  public userName: string = "";
  public password: string = "";

  public mounted() {}

  public login(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) return;

    // register!
    this.$axios
      .post(
        "/api/account/login",
        {
          userName: this.userName,
          password: this.password
        },
        { withCredentials: true }
      )
      .then((response: AxiosResponse<LoginResponse>) => {
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
              solid: true,
              toaster: "b-toaster-bottom-right",
              title: "Welkom!",
              variant: "info"
            });
            this.$router.push({ name: "QuizMasterLobby" });
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
