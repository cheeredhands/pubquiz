import * as SignalR from '@microsoft/signalr';
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
      .withUrl(process.env.VUE_APP_BACKEND_URI + 'gamehub', { accessTokenFactory: () => localStorage.getItem('token') || '' })
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

    // define methods for each server-side call first before starting the hub.
    connection.on('TeamRegistered', data => {
      store.dispatch('processTeamRegistered', data);
    });
    
    connection.on('QmTeamRegistered', data => {
      store.dispatch('processQmTeamRegistered', data);
    });

    connection.on('TeamLoggedOut', data => {
      console.log(data);
      store.dispatch('processTeamLoggedOut', data)
    })

    connection.on('TeamNameUpdated', data => {
      console.log(data);
      store.dispatch('processTeamNameUpdated', data);
    });

    connection.on('TeamMembersChanged', data => {
      console.log(data);
      store.dispatch('processTeamMembersChanged', data);
    })

    connection.on('GameStateChanged', data => {
      console.log(data);
      store.dispatch('processGameStateChanged', data);
    })

    connection.on('TeamDeleted', data => {
      console.log(data);
      store.dispatch('processTeamDeleted', data);
    })

    connection.on('ItemNavigated', data => {
      console.log(data);
      store.dispatch('processItemNavigated', data);
    })

    connection.on('InteractionResponseAdded', data => {
      console.log(data);
      store.dispatch('processInteractionResponseAdded', data);
    })

    connection.on('AnswerScored', data => {
      console.log(data);
      store.dispatch('processAnswerScored', data);
    })
    return start().catch(err => {
      return console.error(err.toString());
    });
  }
};
