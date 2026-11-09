import ModuleBase from '@tmobis/core/modules/module';
import type {ReactNode} from 'react';
import {SignalRProvider} from './react';
import type {SignalRConnectionOptions} from './service';

export type SignalRModuleOptions = SignalRConnectionOptions;
export type {SignalRLifecycleErrorEvent} from './service';

/**
 * React модуль адаптера SignalR для слоя обмена сообщениями.
 * Добавляет React provider для жизненного цикла соединения.
 */
export default class SignalRModule extends ModuleBase {
  private readonly options: SignalRModuleOptions;

  constructor(options: SignalRModuleOptions = {}) {
    super();
    this.options = options;
  }

  wrapProvider(children: ReactNode): ReactNode {
    return (
      <SignalRProvider options={this.options}>
        {children}
      </SignalRProvider>
    );
  }
}
