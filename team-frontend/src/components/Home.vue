<template>
  <div id="content">
    <h1>{{ $t('components.home.WELCOME')}}</h1>
    <router-link
      class="btn btn-primary btn-lg"
      :to="{name: 'RegisterTeam'}">{{ $t('components.home.REGISTERHERE') }}</router-link>
    <hr />
    <router-link :to="{name: 'QuizMasterLogin'}">{{ $t('components.home.QUIZMASTERLOGIN') }}</router-link>
  </div>
</template>

<style scoped>
</style>

