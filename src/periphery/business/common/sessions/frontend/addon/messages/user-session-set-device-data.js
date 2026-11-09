import Message from "@timepath/tmobis-core/messages/message";
import MessageMetadata, { MessageType }
  from "@timepath/tmobis-core/messages/message-metadata";

const messageMetadata = new MessageMetadata(
      MessageType.command, 'UserSessions', 'SetDeviceData');

export default class UserSessionSetDeviceData
extends Message {
  constructor(deviceInfo) {
    super();
    this.deviceInfo = deviceInfo;
  }

  getMetadata() {
    return messageMetadata;
  }
}