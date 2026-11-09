import AntDesign from '@expo/vector-icons/AntDesign'; 
import { VStack } from "../ui/vstack";
import { Text } from "../ui/text";
import { HStack } from "../ui/hstack";
import { Pressable } from "../ui/pressable";
import { Image } from "../ui/image";
import { useState } from "react";
import { FileInfoProps } from './use-file-service';
import { ImageViewer,
  ImageViewerContent,
  ImageViewerCloseButton,
  ImageViewerNavigation,
} from '../ui/image-viewer'

interface FilePreviewProps {
  fileInfo: FileInfoProps;
  downloadFile: (fileInfo: FileInfoProps) => Promise<void>;
  deleteFile: (fileInfo: FileInfoProps) => void;
}

const fileExtensions = new Map([
  ['pdf', 'file-pdf'],
  ['doc', 'file-word'],
  ['docx', 'file-word'],
  ['rtf', 'file-word'],
  ['txt', 'file-text'],
  ['xls', 'file-excel'],
  ['xlsx', 'file-excel'],
  ['default', 'file']
]);

const getIconName = (extension: string) => {
  const name = fileExtensions.has(extension) 
    ? fileExtensions.get(extension): fileExtensions.get('default');
  return <AntDesign name={name as keyof typeof AntDesign.glyphMap} size={73} color="black" />;
};

const FilePreview = ({ fileInfo, downloadFile, deleteFile } : FilePreviewProps) => {
  const [isModalVisible, setIsModalVisible] = useState(false);
  const name = fileInfo.name.toLowerCase();
  const extension = name.includes('.') ? name.split('.').pop() : '';
  const isImageType = (mimeType: string): boolean => {
    return mimeType.startsWith('image/');
  };
  return (
    <HStack>
      <VStack>
        {isImageType(fileInfo.type) ? (
          <Pressable onPress={() => setIsModalVisible(true)}>
            <Image source={{ uri: fileInfo.url }} className="w-full"/>
          </Pressable>
        ) : ( getIconName(extension as string) )}
        <Text>{fileInfo.name}</Text>
      </VStack>
      <VStack>
        <Pressable onPress={() => downloadFile(fileInfo)}>
          <AntDesign name="download" size={20} />
        </Pressable>
        <Pressable onPress={() => deleteFile(fileInfo)}>
          <AntDesign name="delete" size={20} />
        </Pressable>
      </VStack>
      <ImageViewer
        images={[fileInfo]}
        isOpen={isModalVisible}
        onOpenChange={setIsModalVisible}
      >
        <ImageViewerContent>
          <ImageViewerCloseButton />
          <ImageViewerNavigation />
        </ImageViewerContent>
      </ImageViewer>
    </HStack>
  );
};

export { FilePreview };