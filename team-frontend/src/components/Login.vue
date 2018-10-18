<template>
   <div id="content">
    <h1>Quiz master / admin login</h1>
    <div class="login">
    <p><label for="userName">User name</label><input v-model="userName" id="userName" /> </p>
    <p> <label for="password">Password</label> <input v-model="password" id="password"/></p>
 <p><button type="submit" @click="login()">Login</button></p>
 </div>
 <div>
  
  </div>
 </div>
</template>

<script>
export default {
  name: "Login"
};
</script>

<style>
</style>
