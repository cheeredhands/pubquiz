import * as SignalR from "@aspnet/signalr";
import Vuex from "vuex";
import store from "./../store/index.js";

export default {
  init() {

    const connection = new SignalR.HubConnectionBuilder()
      .withUrl("https://localhost:5001/gamehub")
      .build();

    connection
      .start()
      .then(() => {
        // save it.
        store.commit("saveSignalRConnection", connection);
      });

    // define methods for each server-side call.
    connection.on('NotifyTeamRegistered', message => {

    });
  }
};
