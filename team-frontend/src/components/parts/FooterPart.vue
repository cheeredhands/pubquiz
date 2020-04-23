<template>
  <footer class="footer">
    <slot name="footerslot">footer</slot>
  </footer>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';

@Component
export default class FooterPater extends Vue {
    public name: string = 'FooterPart';
}
</script>