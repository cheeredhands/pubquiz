<template>
  <b-navbar toggleable="md" type="dark" variant="dark">
    <b-navbar-brand to="/" :title="$t('HOME_TITLE')">{{ $t('APP_TITLE')}}</b-navbar-brand>
    <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
    <b-collapse is-nav id="nav-collapse">
      <b-navbar-nav>
        <slot></slot>
      </b-navbar-nav>
      <b-navbar-nav align="center" class="ml-auto">
        <b-nav-text>
          <slot name="centercontent"></slot>
        </b-nav-text>
      </b-navbar-nav>
      <!-- Right aligned nav items -->
      <b-navbar-nav class="ml-auto">
        <slot name="rightcontent"></slot>
        <b-nav-item-dropdown v-if="isLoggedIn" :text="userName" right>
          <b-dropdown-item @click="logOut()">{{ $t('LEAVE_GAME')}}</b-dropdown-item>
          <b-dropdown-item>{{ $t('MENU_HELP') }}</b-dropdown-item>
          <b-dropdown-item to="/about">{{ $t('MENU_ABOUT') }}</b-dropdown-item>
        </b-nav-item-dropdown>
      </b-navbar-nav>
    </b-collapse>
  </b-navbar>
</template>

<script lang="ts">
import { AxiosResponse } from 'axios';
import Component, { mixins } from 'vue-class-component';
import AccountServiceMixin from '../../services/account-service-mixin';
// import { ResultCode } from '../../models/ResultCode';
import { ApiResponse } from '../../models/apiResponses';

@Component
export default class NavBarPart extends mixins(AccountServiceMixin) {
  public name = 'NavBarPart';

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
    return this.team.name || this.user.userName;
  }

  public logOut() {
    this.$_accountService_logOutCurrentUser().then(
      (response: AxiosResponse<ApiResponse>) => {
        if (response.data.code === 'LoggedOut') {
          this.$store.dispatch('logout');
          this.$router.replace('/');
        }
      }
    );
  }
}
</script>
