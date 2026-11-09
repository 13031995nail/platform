import React from 'react';
import type { UIMessage } from 'ai';
import type { ListRenderItem } from 'react-native';
import { Message, MessageContent, MessageResponse } from './message';

export const renderConversationMessage: ListRenderItem<UIMessage> = ({ item, index }) => (
  <Message role={item.role} index={index} message={item}>
    <MessageContent>
      <MessageResponse message={item} />
    </MessageContent>
  </Message>
);
