<template>
  <div id="content">
    <b-container>
      <b-row>
        <h1>Registreer hier!</h1>
      </b-row>
      <hr />
      <b-row>
        <b-form @submit="register" novalidate>
          <b-form-group label="Teamnaam:" description="Houd het netjes!" label-for="teamNameInput">
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
            <b-form-invalid-feedback>Een teamnaam van minimaal 5 en maximaal 30 karakters is verplicht.</b-form-invalid-feedback>
          </b-form-group>
          <b-form-group
            label="Code:"
            label-for="codeInput"
            description="De code krijg je van de quizmaster."
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
            <b-form-invalid-feedback>Een code is minimaal 4 tekens.</b-form-invalid-feedback>
          </b-form-group>
          <b-button type="submit" variant="primary">Registreren</b-button>
        </b-form>
      </b-row>
    </b-container>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { AxiosResponse } from "axios";
import { TeamInfo, RegisterForGameResponse } from "../models/models";
import { mixins } from "vue-class-component";
import AccountServiceMixin from "@/services/accountservice";

@Component
export default class RegisterTeam extends mixins(AccountServiceMixin) {
  public name: string = "RegisterTeam";
  public teamName: string = "";
  public code: string = "";

  public mounted() {}

  public register(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) return;
    // // check validation
    // evt.preventDefault();
    // evt.stopPropagation();

    // const form = evt.srcElement as HTMLFormElement;

    // if (form.checkValidity() === false) {
    //   // https://getbootstrap.com/docs/4.3/components/forms/#custom-styles
    //   form.classList.add("was-validated");
    //   console.log("invalid, canceling.");
    //   return;
    // }
    // console.log("valid, registering.");

    // register!
    this.registerForGame(this.teamName, this.code)
      .then((response: AxiosResponse<RegisterForGameResponse>) => {
        this.$store
          .dispatch("storeToken", response.data.jwt)
          .then(() => {
            // disco. init team (add team to store, start signalr)
            this.$store
              .dispatch("initTeam", {
                teamId: response.data.teamId,
                teamName: response.data.teamName,
                memberNames: response.data.memberNames,
                isLoggedIn: true
              })
              .then(() => {
                // and goto lobby
                this.$snotify.success("Welkom!"); // TODO: get message from response
                this.$router.push({ name: "TeamLobby" });
              });
          });
      })
      .catch(error => this.$snotify.error(error.response.data[0].message));
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
