import * as SignalR from "@aspnet/signalr";
import Vuex from "vuex";
import * as store from './../store/';

export default {
  init() {

    const connection = new SignalR.HubConnectionBuilder()
      .withUrl("https://localhost:5001/gamehub")
      .build();

    connection
      .start()
      .then(() => {
        // save it
        this.$store.commit("saveSignalRConnection", connection);
      });

    // define methods for each server-side call.
    connection.on('NotifyTeamRegistered', function (message) {

    });
  }
};
