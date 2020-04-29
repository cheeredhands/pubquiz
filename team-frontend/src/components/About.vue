<template>
  <div id="app">
    <NavBarPart>      
      <template v-slot:centertext>Home</template>
    </NavBarPart>
    <b-container fluid>
      <b-row>
        <b-col>
          <h1>{{ $t('ABOUT_WELCOME')}}</h1>
          <p>text</p>
        </b-col>
      </b-row>
    </b-container>
    <FooterPart>Home footertext</FooterPart>
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
export default class About extends Vue {
  public name: string = 'About';
}
</script>