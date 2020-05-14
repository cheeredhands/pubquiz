<template>
  <div id="app">
    <NavBarPart>{{$t('REGISTER_TEAM')}}</NavBarPart>
    <b-container>
      <b-row>
        <b-col>
          <h1>{{$t('REGISTER_TEAM')}}</h1>
          <hr />
        </b-col>
      </b-row>
      <b-form @submit="register" novalidate>
        <b-form-row>
          <b-col md="6" lg="4">
            <b-form-group
              :label="$t('TEAMNAME')"
              :description="$t('KEEP_IT_CLEAN')"
              label-for="teamNameInput"
              :invalid-feedback="$t('TEAMNAME_LENGTH')"
            >
              <b-form-input
                type="text"
                size="lg"
                v-model="teamName"
                id="teamNameInput"
                name="teamNameInput"
                required
                trim
                minlength="5"
                maxlength="30"
              />
            </b-form-group>
          </b-col>
        </b-form-row>
        <b-form-row>
          <b-col md="6" lg="4">
            <b-form-group
              :label="$t('CODE')"
              label-for="codeInput"
              :description="$t('CODE_ORIGIN')"
              :invalid-feedback="$t('CODE_LENGTH')"
            >
              <b-form-input
                type="text"
                size="lg"
                v-model="code"
                id="codeInput"
                name="codeInput"
                required
                trim
                minlength="4"
              />
            </b-form-group>
          </b-col>
        </b-form-row>
        <b-form-row>
          <b-col>
            <b-button type="submit" variant="primary">{{ $t('REGISTER') }}</b-button>
          </b-col>
        </b-form-row>
      </b-form>
    </b-container>
    <FooterPart />
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import { mixins } from 'vue-class-component';
import { AxiosResponse, AxiosError } from 'axios';
import VueI18n from 'vue-i18n';
import {
  TeamInfo,
  RegisterForGameResponse,
  ApiResponse
} from '../models/models';
import AccountServiceMixin from '../services/account-service-mixin';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';
import HelperMixin from '../services/helper-mixin';

@Component({
  components: { NavBarPart, FooterPart }
})
export default class RegisterTeam extends mixins(AccountServiceMixin, HelperMixin) {
  public name: string = 'RegisterTeam';
  public teamName: string = '';
  public code: string = '';

  // public created() { }

  public register(evt: Event) {
    if (!this.$quizrhelpers.formIsValid(evt)) {
      return;
    }

    this.$_accountService_registerForGame(this.teamName, this.code)
      .then((response: AxiosResponse<RegisterForGameResponse>) => {
        this.$store.dispatch('storeToken', response.data.jwt).then(() => {
          this.$store
            .dispatch('initTeam', {
              teamId: response.data.teamId,
              teamName: response.data.teamName,
              memberNames: response.data.memberNames,
              isLoggedIn: true
            })
            .then(() => {
              this.$router.push({ name: 'TeamLobby' });
            });
        });
      })
      .catch((error: AxiosError<ApiResponse>) => {
       this.$_helper_toastError(error);
      });
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
