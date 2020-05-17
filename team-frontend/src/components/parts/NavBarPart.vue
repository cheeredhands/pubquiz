<template>
  <b-navbar toggleable="md" type="dark" variant="dark">
    <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>
    <b-navbar-brand to="/" :title="$t('HOME_TITLE')">{{ $t('APP_TITLE')}}</b-navbar-brand>
    <b-collapse is-nav id="nav_collapse">
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
import Vue from 'vue';
import { AxiosResponse, AxiosError } from 'axios';
import { UserRole } from '../../models/models';
import Component, { mixins } from 'vue-class-component';
import AccountServiceMixin from '../../services/account-service-mixin';
import { ResultCode } from '../../models/ResultCode';
import { ApiResponse } from '@/models/apiResponses';

@Component
export default class NavBarPart extends mixins(AccountServiceMixin) {
  public name: string = 'NavBarPart';

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
    this.$_accountService_logOutCurrentUser().then(
      (response: AxiosResponse<ApiResponse>) => {
        if (response.data.code === ResultCode.LoggedOut) {
          this.$store.dispatch('logout');
          this.$router.replace('/');
        }
      }
    );
  }
}
</script>