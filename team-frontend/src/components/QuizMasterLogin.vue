<template>
  <div id="app">
    <NavBarPart />
  <b-container fluid>
    <b-row>
      <b-col>
        <h1>{{ $t('QUIZMASTER_LOGIN') }}</h1>
        <hr />
      </b-col>
    </b-row>
    <b-form @submit="login" novalidate>
      <b-form-row>
        <b-col md="6" lg="4">
          <b-form-group :label="$t('USERNAME')" label-for="userNameInput">
            <b-form-input
              type="text"
              size="lg"
              v-model="userName"
              id="userNameInput"
              name="userNameInput"
              required
              minlength="5"
              maxlength="30"
            />
            <b-form-invalid-feedback>{{ $t('USERNAME_LENGTH') }}</b-form-invalid-feedback>
          </b-form-group>
        </b-col>
      </b-form-row>
      <b-form-row>
        <b-col md="6" lg="4">
          <b-form-group :label="$t('PASSWORD')" label-for="passwordInput">
            <b-form-input
              type="password"
              size="lg"
              v-model="password"
              id="passwordInput"
              name="passwordInput"
              required
              minlength="2"
            />
            <b-form-invalid-feedback>{{ $t('PASSWORD_LENGTH') }}</b-form-invalid-feedback>
          </b-form-group>
        </b-col>
      </b-form-row>
      <b-form-row>
        <b-col md="6" lg="4">
          <b-button type="submit" variant="primary">{{ $t('LOGIN') }}</b-button>
        </b-col>
      </b-form-row>
    </b-form>
  </b-container>
      <FooterPart />
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import { AxiosResponse, AxiosError } from 'axios';
import { LoginResponse } from '../models/models';
import Component, { mixins } from 'vue-class-component';
import AccountServiceMixin from '../services/accountservice';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';

@Component({
  components: { NavBarPart, FooterPart }
})
export default class QuizMasterLogin extends mixins(AccountServiceMixin) {
  public name: string = 'QuizMasterLogin';
  public userName: string = '';
  public password: string = '';

  public created() {
    this.$store.commit('setNavbarText', 'Quiz master login');
  }

  public login(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) { return; }
    this.loginUser(this.userName, this.password)
      .then((response: AxiosResponse<LoginResponse>) => {
        this.$store.dispatch('storeToken', response.data.jwt).then(() => {
          this.$store
            .dispatch('initQuizMaster', {
              userId: response.data.userId,
              userName: response.data.userName,
              gameIds: response.data.gameIds,
              isLoggedIn: true
            })
            .then(() => {
              // and goto lobby
              this.$router.push({ name: 'QuizMasterLobby' });
            });
        });
      })
      .catch((error: AxiosError) => {
        this.$bvToast.toast(error.message, {
          title: this.$t('ERROR_MESSAGE_TITLE').toString(),
          variant: 'error'
        });
      });
  }
}
</script>

<style>
</style>
