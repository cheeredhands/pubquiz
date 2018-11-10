<template>
  <div id="app">
    <div id="nav">
      <router-link to="/">Home</router-link> |
      <router-link to="/about">About</router-link>
    </div>
    <router-view/>
    <footer class="footer">{{message}}</footer>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { AxiosResponse } from 'axios';
import { WhoAmIResponse } from '@/models/models';

@Component
export default class App extends Vue {
  name: string = "app";

  message: string = "";

  mounted() {
    this.$axios
      .get('/api/account/whoami', { withCredentials: true })
      .then((response: AxiosResponse<WhoAmIResponse>) => {
        if (response.data.userName === '') {
          return;
        }
        // disco. init team (add team to store, start signalr)
        this.$store
          .dispatch('initTeam', {
            teamId: response.data.userId,
            teamName: response.data.userName,
          })
          .then(() => {
            // and goto lobby
            this.$router.replace('Lobby');
          });
      })
      .catch(error => (this.message = error.response));
  }
}
</script>

<style>
html,
body {
  height: 100%;
}
body {
  margin: 0;
}
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  /* margin: 0px; */
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 50px 1fr 50px;
  height: 100%;
}
#nav {
  background-color: aliceblue;
  padding: 30px;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
}

#nav a.router-link-exact-active {
  color: #42b983;
}

footer {
  background-color: aliceblue;
  font-size: 10px;
}

#content {
  display: grid;
  grid-template-rows: 1fr 2fr 20px;
}
</style>
