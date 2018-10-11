<template>
  <div class="register">
    <h1>{{ msg }}</h1>
    <p><label for="teamName">Team name</label><input v-model="teamName" id="teamName" /> </p>
    <p> <label for="code">Code</label> <input v-model="code" id="code"/></p>
 <p><button type="submit" @click="register()">Register</button></p>
  </div>
</template>

<script>
import Axios from "axios";

export default {
  name: "RegisterTeam",
  props: {
    msg: String
  },
  data() {
    return {
      teamName: "",
      code: ""
    };
  },
  methods: {
    register() {
      // register!
      Axios.post(
        "/api/account/register",
        {
          teamName: this.teamName,
          code: this.code
        },
        { withCredentials: true }
      )
        .then(response => {
          // disco. add team to store
          this.$store.commit(
            "setTeam",
            { teamId: response.data.teamId, teamName: this.teamName }
          );

          // and goto lobby
          this.$router.push("Lobby");
        })
        .catch(error => (this.msg = error.response.data[0].message));
    }
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
