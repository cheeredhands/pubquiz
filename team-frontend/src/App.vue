<template>
  <router-view></router-view>
</template>

<script lang="ts">
import Vue from 'vue';
import { AxiosResponse, AxiosError } from 'axios';
import Component, { mixins } from 'vue-class-component';
import { WhoAmIResponse, UserRole } from './models/models';
import { ResultCode } from './models/ResultCode';
import AccountServiceMixin from './services/accountservice';
import NavBarPart from './components/parts/NavBarPart.vue';
import FooterPart from './components/parts/FooterPart.vue';

@Component({
  components: { NavBarPart, FooterPart }
})
export default class App extends mixins(AccountServiceMixin) {
  public name: string = 'App';

  public mounted() {
    this.getWhoAmI()
      .then((response: AxiosResponse<WhoAmIResponse>) => {
        if (response.data.code === ResultCode.LoggedOut) {
          this.$store.dispatch('logout');
          return;
        }
        if (response.data.userRole === UserRole.Team) {
          this.$store
            .dispatch('initTeam', {
              teamId: response.data.userId,
              teamName: response.data.userName,
              currentGameId: response.data.currentGameId
            })
            .then(() => {
              this.$router.replace({ name: 'TeamLobby' });
            });
        } else {
          this.$store
            .dispatch('initQuizMaster', {
              userId: response.data.userId,
              userName: response.data.userName
            })
            .then(() => {
              this.$router.replace({ name: 'QuizMasterLobby' });
            });
        }
      })
      .catch((error: AxiosError) => {
        this.$bvToast.toast(error.message, {
          title: 'oops',
          variant: 'error'
        });
      });
  }
}
</script>

<style>
@import "~bootstrap/dist/css/bootstrap.css";
@import "~bootstrap-vue/dist/bootstrap-vue.css";

#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 50px 1fr 50px;
  grid-template-areas: "Nav" "Main" "Footer";
  height: 100vh;
}

nav a {
  font-weight: bold;
  color: aliceblue;
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
  /* font-size: 0.7rem; */
  padding: 0.5em;
}

.strong {
  font-style: italic;
}
</style>
