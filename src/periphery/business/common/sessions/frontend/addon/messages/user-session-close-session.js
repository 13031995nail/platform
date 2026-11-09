import Message from "@timepath/tmobis-core/messages/message";
import MessageMetadata, { MessageType }
  from "@timepath/tmobis-core/messages/message-metadata";

const messageMetadata = new MessageMetadata(
      MessageType.command, 'UserSessions', 'StopSession');

export default class UserSessionStopSession
extends Message {
  constructor(sessionId) {
    super();
    this.sessionId = sessionId;
  }

  getMetadata() {
    return messageMetadata;
  }
}