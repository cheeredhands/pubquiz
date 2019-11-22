<template>
  <div id="content">
    <h1>{{ $t('HOME_WELCOME')}}</h1>
    <router-link
      class="btn btn-primary btn-lg"
      :to="{name: 'RegisterTeam'}">{{ $t('HOME_REGISTERHERE') }}</router-link>
    <hr />
    <router-link :to="{name: 'QuizMasterLogin'}">{{ $t('HOME_QUIZMASTERLOGIN') }}</router-link>
  </div>
</template>

<style scoped>
</style>

