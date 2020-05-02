<template>
  <div id="app">
    <NavBarPart>{{$t('HOME_NAVTITLE')}}</NavBarPart>
    <b-container>
      <b-row>
        <b-col>
          <h1>{{ $t('HOME_WELCOME')}}</h1>
          <b-button
            variant="primary"
            size="lg"
            :to="{name: 'RegisterTeam'}"
          >{{ $t('HOME_REGISTERHERE') }}</b-button>
        </b-col>
      </b-row>
    </b-container>
    <FooterPart>
      Home footer text
      <template v-slot:footeractions>
        <b-link :to="{name: 'QuizMasterLogin'}">{{ $t('HOME_QUIZMASTERLOGIN') }} |</b-link>
      </template>
    </FooterPart>
  </div>
</template>
    </FooterPart>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import NavBarPart from './parts/NavBarPart.vue';
import FooterPart from './parts/FooterPart.vue';

@Component({
  components: { NavBarPart, FooterPart }
})
export default class Home extends Vue {
  public name: string = 'Home';
}
</script>