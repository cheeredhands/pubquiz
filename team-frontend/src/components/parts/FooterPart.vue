<template>
  <footer class="footer font-small">
    <slot>footer</slot>
    
    <b-link class="float-right" :to="{name: 'About'}">| {{ $t('MENU_ABOUT') }}</b-link>
    
    <div class="float-right">
      <slot name="footeractions"></slot>
    </div>
  </footer>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';

@Component
export default class FooterPart extends Vue {
  public name: string = 'FooterPart';
}
</script>