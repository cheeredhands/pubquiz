<template>
  <div id="content">
    <h1>{{ msg }}</h1>
    <div class="login">
    <p><label for="teamName">Team name</label><input v-model="teamName" id="teamName" /> </p>
    <p> <label for="code">Code</label> <input v-model="code" id="code"/></p>
 <p><button type="submit" @click="register()">Register</button></p>
 </div>
 <div>
  <router-link to="Login">Admin</router-link>
  </div>
 </div>
</template>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { Prop } from "vue-property-decorator";
import { AxiosResponse } from "axios";
import { TeamInfo } from "../models/models";

@Component({})
export default class RegisterTeam extends Vue {
  @Prop()
  private msg!: string;
  name: string = "RegisterTeam";
  teamName: string = "";
  code: string = "JOINME";

  register() {
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
        this.$store.dispatch("initTeam", {
          teamId: response.data.teamId,
          teamName: this.teamName
        });

        // and goto lobby
        this.$router.push("Lobby");
      })
      // TODO: put catch above then???
      .catch(error => (this.msg = error.response.data[0].message));
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
