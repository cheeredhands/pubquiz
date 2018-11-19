<template>
  <div id="app">
    <b-navbar toggleable="md" type="dark" variant="dark">
      <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>
      <b-navbar-brand href="#">Quizr</b-navbar-brand>
      <b-collapse is-nav id="nav_collapse">
      <b-navbar-nav>
        <b-nav-item><router-link to="/">Home</router-link></b-nav-item>
      </b-navbar-nav>     
       <!-- Right aligned nav items -->
      <b-navbar-nav class="ml-auto">
        <b-nav-item><router-link to="/about">About</router-link></b-nav-item>
      </b-navbar-nav>
      </b-collapse>     
    </b-navbar>
    <router-view/>
    <footer class="footer">{{message}}</footer>
     <vue-snotify></vue-snotify>
 </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { AxiosResponse } from 'axios';
import { WhoAmIResponse } from './models/models';

@Component
export default class App extends Vue {
  public name: string = 'app';

  public message: string = '';

  public mounted() {
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
            teamName: response.data.userName
          })
          .then(() => {
            // and goto lobby
            this.$router.replace('Lobby');
          });
      })
      .catch(error => this.$snotify.error(error.message));
  }
}
</script>

<style>
@import '~vue-snotify/styles/material.css';
@import '~bootstrap/dist/css/bootstrap.css';
@import '~bootstrap-vue/dist/bootstrap-vue.css';

html,
body {
  height: 100%;
}
body {
  margin: 0;
}
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  /* text-align: center; */
  color: #2c3e50;
  /* margin: 0px; */
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 50px 1fr 50px;
  height: 100%;
}

nav a {
  font-weight: bold;
  color: gainsboro;
}

nav a:hover {
  color: white;
  text-decoration: none;
}

nav a.router-link-exact-active {
  color: white;
}

footer {
  background-color: lightblue;
  font-size: 10px;
}

#content {
  padding: 10px;
  /* display: grid;
  grid-template-rows: 1fr 2fr 20px; */
}
</style>
