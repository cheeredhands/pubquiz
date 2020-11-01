<template>
  <div id="main">
    <nav-bar-part>
      <template v-slot:centertext>Home</template>
    </nav-bar-part>
    <div class="main-container">
      <b-container>
        <b-row>
          <b-col>
            <h1>{{ $t('ABOUT_WELCOME')}}</h1>
            <p>text</p>
          </b-col>
        </b-row>
      </b-container>
    </div>
    <footer-part>Home footertext</footer-part>
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
  public name = 'About';
}
</script>
