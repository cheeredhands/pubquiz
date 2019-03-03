import * as SignalR from '@aspnet/signalr';
import store from '../store/index';

export default {

  close() {
    const connection = store.state.signalrconnection;
    if (connection !== undefined) {
      connection.stop().then(() => {
        store.commit('clearSignalRConnection');
      });
    }
  },
  init() {
    const connection = new SignalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/gamehub')
      .configureLogging(SignalR.LogLevel.Information)
      .build();

    async function start() {
      try {
        await connection.start();
        store.commit('saveSignalRConnection', connection);
        console.log("connected");
      } catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
      }
    };

    connection.onclose(async () => {
      await start();
    });

    // function connect(conn: SignalR.HubConnection) {
    //   return conn.start().catch(e => {
    //     //sleep(5000);
    //     //console.log(`Reconnecting Socket because of ${e}`); // tslint:disable-line no-console
    //     //connect(conn);
    //   });
    // }

    //connection.onclose(() => {
    //connect(connection);
    //});

    // TODO: refactor this into helper class.
    // function sleep(milliseconds: number) {
    //   const start = new Date().getTime();
    //   for (let i = 0; i < 1e7; i++) {
    //     if (new Date().getTime() - start > milliseconds) {
    //       break;
    //     }
    //   }
    // }

    // define methods for each server-side call first before starting the hub.
    connection.on('TeamRegistered', data => {
      store.dispatch('processTeamRegistered', data);
    });

    connection.on('TeamNameUpdated', data => {
      console.log(data); // tslint:disable-line no-console
      store.dispatch('renameOtherTeam', data);
    });

    return start().catch(function (err) {
      return console.error(err.toString());
    });


    // connect(connection).then(() => {
    //   // save it
    //   store.commit('saveSignalRConnection', connection);
    // });
  }
};
