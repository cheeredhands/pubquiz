<template>
  <div id="app">    
    <nav class="nav"></nav>
    <router-view/>
    <footer class="footer">{{message}}</footer>
  </div>
</template>

<script>
export default {
  name: "app",
  data() {
    return {
      message: ""
    };
  },
  created() {
    this.$axios
      .get("/api/account/whoami", { withCredentials: true })
      .then(response => {
        if (response.data.userName === "") {
          return;
        }
        // disco. init team (add team to store, start signalr)
        this.$store
          .dispatch("initTeam", {
            teamId: response.data.userId,
            name: response.data.userName
          })
          .then(() => {
            // and goto lobby
            this.$router.replace("Lobby");
          });
      })
      .catch(error => (this.message = error.response));
  }
};
</script>

<style>
html,
body {
  height: 100%;
}
body {
  margin: 0;
}

#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  /* margin: 0px; */
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 50px 1fr 50px;
  height: 100%;
}
nav {
  background-color: aliceblue;
}
footer {
  background-color: aliceblue;
}

#content {
  display: grid;
  grid-template-rows: 1fr 2fr 20px;
}
</style>
