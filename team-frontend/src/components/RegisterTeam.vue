<template>
  <div id="content">
    <b-container>
      <b-row>
        <h1>Registreer hier!</h1>
      </b-row>
      <hr>
      <b-row>
        <b-form @submit="submitForm">
          <b-form-group label="Teamnaam:" description="Houd het netjes!" label-for="teamNameInput">
            <b-form-input
              v-validate="'required|min:5|max:30'"
              type="text"
              size="lg"
              v-model="teamName"
              id="teamNameInput"
              name="teamNameInput" required minlength="5"
            />
            <b-form-invalid-feedback>Een teamnaam van minimaal 5 en maximaal 30 karakters is verplicht.</b-form-invalid-feedback>
          </b-form-group>
          <b-form-group
            label="Code:"
            label-for="codeInput"
            description="De code krijg je van de quizmaster."
          >
            <b-form-input
              v-validate="'required|min:4'"
              type="text"
              size="lg"
              v-model="code"
              id="codeInput"
              name="codeInput"
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
import { TeamInfo } from "../models/models";
import { Form } from 'bootstrap-vue';

@Component
export default class RegisterTeam extends Vue {
  public name: string = "RegisterTeam";
  public teamName: string = "";
  public code: string = "";
  private submitted: boolean = false;

  get teamNameValidation() {
    if (!this.submitted) return "null";
    return this.teamName.length > 4 && this.teamName.length < 31;
  }

  get codeValidation() {
    if (!this.submitted) return "null";
    return this.code.length > 3;
  }

  public mounted() {}

  public submitForm(evt: Event) {
    evt.preventDefault();
    const form = evt.srcElement as HTMLFormElement;
    form.classList.add('was-validated');
    form.reportValidity();
    
   // form.validated = '';
    if (form.checkValidity()) {
      alert(form.checkValidity());
    }
    
  }

  public register() {
    // validate
    this.submitted = true;
    // register!
    this.$axios
      .post(
        "/api/account/register",
        {
          teamName: this.teamName,
          code: this.code
        },
        { withCredentials: true }
      )
      .then((response: AxiosResponse<TeamInfo>) => {
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
            this.$router.push("Lobby");
          });
      })
      .catch(error => this.$snotify.error(error.response.data[0].message));
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
