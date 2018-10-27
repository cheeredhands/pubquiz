import * as SignalR from "@aspnet/signalr";
import store from "./../store/index.js";

export default {
  init() {
    const connection = new SignalR.HubConnectionBuilder()
      .withUrl("https://localhost:5001/gamehub")
      .configureLogging(SignalR.LogLevel.Information)
      .build();

    function connect(conn) {
      return conn.start().catch(e => {
        sleep(5000);
        console.log("Reconnecting Socket because of " + e); // eslint-disable-line no-console
        connect(conn);
      });
    }

    connection.onclose(function () {
      connect(connection);
    });

    // TODO: rafactor this into helper class.
    function sleep(milliseconds) {
      var start = new Date().getTime();
      for (var i = 0; i < 1e7; i++) {
        if (new Date().getTime() - start > milliseconds) {
          break;
        }
      }
    }

    // define methods for each server-side call first before starting the hub.
    connection.on("TeamRegistered", data => {
      store.dispatch("processTeamRegistered", data);
    });

    connection.on("TeamNameUpdated", data => {
      store.dispatch("renameOtherTeam",  data.teamId, data.teamName);
    })

    connect(connection).then(() => {
      // save it
      store.commit("saveSignalRConnection", connection);
    });
  }
};
