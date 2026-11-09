import React, { ReactNode } from 'react';
import {
  Drawer,
  DrawerBackdrop,
  DrawerContent,
  DrawerBody,
} from '@tmobis/core/components/ui/drawer';
import { StyleSheet } from 'react-native';



interface SidebarProps {
  isVisible: boolean;
  onClose: () => void;
  children: ReactNode;
}

const Sidebar = ({ isVisible, onClose, children }: SidebarProps) => {
  return (
    <Drawer
      isOpen={isVisible}
      onClose={onClose}
      anchor="right"
      size="sm"
    >
      <DrawerBackdrop />
      <DrawerContent style={styles.sidebar}>
        <DrawerBody>
          {children}
        </DrawerBody>
      </DrawerContent>
    </Drawer>
  );
};

const styles = StyleSheet.create({
  sidebar: {
    padding: 20,
  },
});

export default Sidebar;
