/*global signalR*/
import config from 'ember-get-config';
import Service, { inject as service } from '@ember/service';

export default Service.extend({
  session: service(),
  /**
  Ограничение на длительность интервала повтора в секундах
  */
  maxDelay: 3600,

  /**
  Начальный интервал повтора в секундах
  */
  firstDelay: 2,

  /**
  Определяет случайную составляющую
  */
  delayRandomCoef: 0.1,

  connection: undefined,

  init() {
    this._super(...arguments);

    let hubUrl = config.APP.backendUrls.signalr;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => {
          const token = this.session.data.authenticated.access_token;
          if (!token) {
            throw new Error("Access token is missing");
          }
          return token;
        }
      })
      .withAutomaticReconnect({nextRetryDelayInMilliseconds: retryContext => {
        let prevDelay = retryContext.elapsedMilliseconds,
          prevRetryCount = retryContext.previousRetryCount,
          nextDelay = prevDelay + Math.min(prevDelay * (Math.pow(2, prevRetryCount) - 1), this.maxDelay * 1000);
        return nextDelay + (Math.random() - 0.5) * 2 * nextDelay * this.delayRandomCoef;
      }})
      .configureLogging(signalR.LogLevel.Information)
      .build();
  },

  async start() {
    try {
      await this.connection.start();
      console.log(`Подключено к ${this.connection.baseUrl}`);
    }
    catch(err) {
      console.error('Ошибка подключения:', err);
    }
  },

  async send(message, type) {
    try {
      const messageWithTypeInfo = type 
        ? Object.assign({}, message, { $type: type }) 
        : message;
      const stringifiedMessage = JSON.stringify(messageWithTypeInfo);
      return await this.connection.invoke("SendMessage", stringifiedMessage);
    }
    catch(err) {
      console.error('Ошибка отправки:', err);
      throw err;
    }
  },

  on(eventType, handler) {
    try {
      this.connection.on(eventType, (rawData) => {
        let parsedData = rawData;
  
        if (typeof rawData === 'string') {
          try {
            parsedData = JSON.parse(rawData);
          } catch (e) {
            console.warn(`Не удалось распарсить JSON: ${rawData}`, e);
          }
        }
  
        handler(parsedData);
      });
    } catch (err) {
      console.error('Ошибка прослушивания:', err);
    }
  },

  async stop() {
    try {
      await this.connection.stop();
      console.log('Соединение закрыто');
    }
    catch(err) {
      console.error('Ошибка остановки подключения:', err);
    }
  },
})