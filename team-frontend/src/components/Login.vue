<template>
  <div id="content">
    <b-container>
      <b-row>
        <h1>Quiz master / admin login</h1>
      </b-row>
      <hr />
      <b-row>
        <b-form @submit="register" novalidate>
          <b-form-group label="username:" label-for="userNameInput">
            <b-form-input
              type="text"
              size="lg"
              v-model="userName"
              id="userNameInput"
              name="userNameInput"
              required
              minlength="5"
              maxlength="30"
            />
          </b-form-group>
          <b-form-group label="Password:" label-for="passwordInput">
            <b-form-input
              type="password"
              size="lg"
              v-model="password"
              id="passwordInput"
              name="passwordInput"
              required
              minlength="4"
            />
          </b-form-group>
          <b-button type="submit" @click="login()" variant="primary">Inloggen</b-button>
        </b-form>
      </b-row>
    </b-container>
    <div></div>
  </div>
</template>

<script lang="ts">
import Vue from "vue";

export default class Login extends Vue {
  public name: string = "Login";
}
</script>

<style>
</style>
