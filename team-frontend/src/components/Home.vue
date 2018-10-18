<template>
  <div id="content">    
      <h1>Welkom bij Quizr!</h1>
      <router-link :to="{name: 'Register', params: { msg: 'Hello new team!' } }">Registreer hier</router-link>
      <router-link to="Login">Admin</router-link>
  </div>
</template>


<script>
export default {};
</script>

<style scoped>
</style>

