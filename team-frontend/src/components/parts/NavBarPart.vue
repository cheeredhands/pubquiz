<template>
  <b-navbar toggleable="sm" type="light" variant="info">
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
        <b-nav-item-dropdown v-if="isLoggedIn" right>
           <template #button-content>
            <em>{{userName}}</em>
          </template>
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
import { Team, User } from '../../models/models';

@Component
export default class NavBarPart extends mixins(AccountServiceMixin) {
  public name = 'NavBarPart';

  get isLoggedIn(): boolean {
    return this.$store.state.isLoggedIn || false;
  }

  get team(): Team {
    return this.$store.getters.team || {};
  }

  get user(): User {
    return this.$store.getters.user || {};
  }

  get userName(): string {
    return this.user.userName || this.team.name;
  }

  public logOut(): void {
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
