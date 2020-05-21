<template>
  <router-view></router-view>
</template>

<script lang="ts">
import Vue from 'vue';
import { AxiosResponse, AxiosError } from 'axios';
import Component, { mixins } from 'vue-class-component';
import { UserRole, Team, User } from './models/models';
import { ResultCode } from './models/ResultCode';
import AccountServiceMixin from './services/account-service-mixin';
import NavBarPart from './components/parts/NavBarPart.vue';
import FooterPart from './components/parts/FooterPart.vue';
import HelperMixin from './services/helper-mixin';
import { WhoAmIResponse } from './models/apiResponses';

@Component({
  components: { NavBarPart, FooterPart }
})
export default class App extends mixins(AccountServiceMixin, HelperMixin) {
  public name: string = 'App';

  public mounted() {
    this.$_accountService_getWhoAmI()
      .then((response: AxiosResponse<WhoAmIResponse>) => {
        if (response.data.code === ResultCode.LoggedOut) {
          this.$store.dispatch('logout');
          return;
        }
        if (response.data.userRole === UserRole.Team) {
          this.$store
            .dispatch('initTeam', {
              teamId: response.data.userId,
              teamName: response.data.teamName,
              memberNames: response.data.memberNames,
              currentGameId: response.data.currentGameId,
              isLoggedIn: true
            } as Team)
            .then(() => {
              this.$router.replace({ name: 'TeamLobby' });
            });
        } else {
          this.$store
            .dispatch('initQuizMaster', {
              userId: response.data.userId,
              userName: response.data.userName
            } as User)
            .then(() => {
              this.$router.replace({ name: 'QuizMasterLobby' });
            });
        }
      })
      .catch((error: AxiosError) => {
        this.$_helper_toastError(error);
      });
  }
}
</script>

<style>
@import "~bootstrap/dist/css/bootstrap.css";
@import "~bootstrap-vue/dist/bootstrap-vue.css";

/* body {
  overflow: hidden;
} */

#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  /* font-size: 0.8rem; */
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: auto 1fr 25px;
  grid-template-areas: "Nav" "Main" "Footer";
  height: 100vh;
  /* https://stackoverflow.com/questions/37112218/css3-100vh-not-constant-in-mobile-browser */
  /* https://news.ycombinator.com/item?id=21103735 */
  min-height: -webkit-fill-available;
}

.main-container {
  overflow: auto;
}

footer {
  background-color: #212529;
  color: white;
  font-size: 0.7rem;
  padding: 0.5em;
}

.strong {
  font-style: italic;
}
</style>
