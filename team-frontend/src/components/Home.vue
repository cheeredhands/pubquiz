<template>
  <div id="content">
      <h1>Welkom bij Quizr!</h1>
      <router-link class="btn btn-primary btn-lg" :to="{name: 'RegisterTeam', params: { msg: 'Hello new team!' } }">Registreer hier</router-link>
      <hr>
      <router-link :to="{name: 'QuizMasterLogin'}">Quizmasters</router-link>
  </div>
</template>

<style scoped>
</style>
