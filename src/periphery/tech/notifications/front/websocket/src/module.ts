import ModuleBase from '@tmobis/core/modules/module';
import {createElement, type ReactNode} from 'react';
import {NotificationWebSocketProvider} from './provider';

/**
 * React модуль доставки оповещений через WebSocket/SignalR.
 * Добавляет React provider.
 */
export default class NotificationWebSocketModule extends ModuleBase {
  wrapProvider(children: ReactNode): ReactNode {
    return createElement(NotificationWebSocketProvider, null, children);
  }
}
