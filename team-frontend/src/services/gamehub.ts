import * as SignalR from '@aspnet/signalr';
import store from '../store/index';

export default {
  closing: false,
  async close() {
    const connection = store.state.signalrconnection;
    if (connection !== undefined) {
      this.closing = true;
      await connection.stop().then(() => {
        store.commit('clearSignalRConnection');
      });
    }
  },
  init() {
    const connection = new SignalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/gamehub')
      .configureLogging(SignalR.LogLevel.Information)
      .build();
    this.closing = false;
    async function start() {
      try {
        await connection.start();
        store.commit('saveSignalRConnection', connection);
        console.log('connected');
      } catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
      }
    };

    connection.onclose(async () => {
      if (!this.closing) {
        await start();
      }
    });

    // function connect(conn: SignalR.HubConnection) {
    //   return conn.start().catch(e => {
    //     //sleep(5000);
    //     //console.log(`Reconnecting Socket because of ${e}`); // tslint:disable-line no-console
    //     //connect(conn);
    //   });
    // }

    // connection.onclose(() => {
    // connect(connection);
    // });

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

    connection.on('TeamLoggedOut', data => {
      console.log(data);
      store.dispatch('processTeamLoggedOut', data)
    })

    connection.on('TeamNameUpdated', data => {
      console.log(data); 
      store.dispatch('processTeamNameUpdated', data);
    });

    connection.on('TeamMembersChanged', data =>{
      console.log(data);
      store.dispatch('processTeamMembersChanged', data);
    })

    return start().catch(err => {
      return console.error(err.toString());
    });


    // connect(connection).then(() => {
    //   // save it
    //   store.commit('saveSignalRConnection', connection);
    // });
  }
};
