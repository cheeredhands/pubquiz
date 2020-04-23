<template>
  <b-navbar toggleable="md" type="dark" variant="dark">
    <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>
    <b-navbar-brand href="#">{{ $t('APP_TITLE')}}</b-navbar-brand>
    <b-collapse is-nav id="nav_collapse">
      <b-navbar-nav>
        <b-nav-item>
          <b-link to="/">{{ $t('MENU_HOME') }}</b-link>
        </b-nav-item>
      </b-navbar-nav>
      <b-navbar-nav class="ml-auto">
        <b-nav-text center>{{ navbarText }}</b-nav-text>
      </b-navbar-nav>

      <!-- Right aligned nav items -->
      <b-navbar-nav class="ml-auto">
        <b-nav-item-dropdown v-if="isLoggedIn" :text="userName" right>
          <b-dropdown-item @click="logOut()">{{ $t('LEAVE_GAME')}}</b-dropdown-item>
          <b-dropdown-item>{{ $t('MENU_HELP') }}</b-dropdown-item>
        </b-nav-item-dropdown>
      </b-navbar-nav>
    </b-collapse>
  </b-navbar>
</template>

<script lang="ts">
import Vue from 'vue';
import { AxiosResponse, AxiosError } from 'axios';
import { WhoAmIResponse, ApiResponse, UserRole } from '../models/models';
import Component, { mixins } from 'vue-class-component';
import AccountServiceMixin from '@/services/accountservice';

@Component
export default class NavBarPart extends mixins(AccountServiceMixin) {
  public name: string = 'NavBarPart';

  get navbarText() {
    return this.$store.state.navbarText || '';
  }
  get isLoggedIn() {
    return this.$store.state.isLoggedIn || false;
  }

  get team() {
    return this.$store.state.team || '';
  }

  get user() {
    return this.$store.state.user || '';
  }

  get userName() {
    return this.team.teamName || this.user.userName;
  }

  public logOut() {
    this.logOutCurrentUser().then((response: AxiosResponse<ApiResponse>) => {
      if (response.data.code === 2) {
        this.$store.dispatch('logout');
        this.$router.replace('/');
      }
    });
  }
}
</script>