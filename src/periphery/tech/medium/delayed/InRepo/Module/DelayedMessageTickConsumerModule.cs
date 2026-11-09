namespace Tmobis.Medium.Delayed.InRepo.Module;

using System;
using System.Collections.Generic;
using Tmobis.Core.Modules;
using Tmobis.Core.Medium.Modules;
using Tmobis.Jobs.Scheduled;

/// <summary>
/// Модуль персистентного хранения отложенных сообщений.
/// Содержит периодическую обработку отложенных сообщений.
/// </summary>
public sealed class DelayedMessageTickConsumerModule :
  ModuleBase,
  IMessageModule,
  IScheduledMessagesModule,
  IConsumerModule
{
  public List<Type> GetScheduledMessages() =>
  [
      typeof(DelayedMessageTick)
  ];
}
