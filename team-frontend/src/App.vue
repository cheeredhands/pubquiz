<template>
  <div id="app">
    <b-navbar toggleable="md" type="dark" variant="dark">
      <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>
      <b-navbar-brand href="#">{{ $t('APP_TITLE')}}</b-navbar-brand>
      <b-collapse is-nav id="nav_collapse">
        <b-navbar-nav>
          <b-nav-item>
            <router-link to="/">{{ $t('MENU_HOME') }}</router-link>
          </b-nav-item>
        </b-navbar-nav>
        <!-- Right aligned nav items -->
        <b-navbar-nav v-if="isLoggedIn" class="ml-auto">
          <b-nav-item-dropdown :text="userName" right>
            <b-dropdown-item @click="logout()">{{ $t('LEAVE_GAME')}}</b-dropdown-item>
            <b-dropdown-item>{{ $t('MENU_HELP') }}</b-dropdown-item>
          </b-nav-item-dropdown>
        </b-navbar-nav>
      </b-collapse>
    </b-navbar>
    <router-view></router-view>
    <footer class="footer">{{message}}</footer>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { AxiosResponse, AxiosError } from "axios";
import Component, { mixins } from "vue-class-component";
import { WhoAmIResponse, ApiResponse, UserRole } from "./models/models";
import AccountServiceMixin from "./services/accountservice";

@Component
export default class App extends mixins(AccountServiceMixin) {
  public name: string = "app";

  public message: string = "Welkom bij Quizr";

  get isLoggedIn() {
    return this.$store.state.isLoggedIn || false;
  }

  get team() {
    return this.$store.state.team || "";
  }

  get user() {
    return this.$store.state.user || "";
  }

  get userName() {
    return this.team.teamName || this.user.userName;
  }

  public logOut() {
    this.logOutCurrentUser()
      .then((response: AxiosResponse<ApiResponse>) => {
        if (response.data.code === 2) {
          this.$store.dispatch("logout");
          this.$router.replace("/");
        }
      });
  }

  public mounted() {
    this.getWhoAmI()
      .then((response: AxiosResponse<WhoAmIResponse>) => {
        if (response.data.userName === "") {
          return;
        }
        if (response.data.userRole === UserRole.Team) {
          this.$store
            .dispatch("initTeam", {
              teamId: response.data.userId,
              teamName: response.data.userName
            })
            .then(() => {
              this.$router.replace({ name: "TeamLobby" });
            });
        } else {
          this.$store
            .dispatch("initQuizMaster", {
              userId: response.data.userId,
              userName: response.data.userName
            })
            .then(() => {
              this.$router.replace({ name: "QuizMasterLobby" });
            });
        }
      })
      .catch((error: AxiosError) => {
        this.$bvToast.toast(error.message, {
          title: "oops",
          variant: "error"
        });
      });
  }
}
</script>

<style>
@import "~bootstrap/dist/css/bootstrap.css";
@import "~bootstrap-vue/dist/bootstrap-vue.css";

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

.strong {
  font-style: italic;
}
</style>
