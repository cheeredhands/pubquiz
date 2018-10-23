import * as SignalR from "@aspnet/signalr";
import Vuex from "vuex";
import store from "./../store/index.js";

export default {
  init() {

    const connection = new SignalR.HubConnectionBuilder()
      .withUrl("https://localhost:5001/gamehub")
      .build();

    // define methods for each server-side call first before starting the hub.
    connection.on('NotifyTeamRegistered', message => {
      store.dispatch("addTeam", message)
    });

    connection
      .start()
      .then(() => {
        // save it. No idea why though...
        store.commit("saveSignalRConnection", connection);
      });

  }
};
