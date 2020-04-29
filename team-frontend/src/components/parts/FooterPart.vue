<template>
  <footer class="footer">
    <slot>footer</slot>
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