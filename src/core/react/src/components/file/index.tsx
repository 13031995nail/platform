import { AntDesign } from '@expo/vector-icons';
import { TmobisButton } from "../button";
import { Box } from "../ui/box";
import { HStack } from "../ui/hstack";
import { Pressable } from "../ui/pressable";
import { FilePreview } from './file-preview';
import useFileService, { FileInfoProps } from "./use-file-service";
import { useEntityEditController } from '@tmobis/core/components/entity-edit/context';
import { TmobisFormControlLabel } from '@tmobis/core/components/form-control';
import { useCallback } from 'react';

export interface FileComponentProps {
	fieldName?: string;
	label?: string;
}

const FileComponent = ({ fieldName, label }: FileComponentProps) => {
	const controller = useEntityEditController({
		fieldName,
	});

	const {
		fileInfos,
		downloadFile,
		pickDocuments,
		deleteFile
	} = useFileService({
		value: controller?.field.value as string,
		onChange: controller?.field.onChange,
	});

	const handleDelete = useCallback((fileInfo: FileInfoProps) => {
		deleteFile(fileInfo);
	}, [deleteFile]);

	return (
		<>
			<TmobisFormControlLabel label={label} />
			<HStack>
				{fileInfos.map((fileInfo) => (
					<Box key={fileInfo.id} position="relative" width="100%" mb={4}>
						<FilePreview fileInfo={fileInfo} downloadFile={downloadFile} deleteFile={handleDelete} />
					</Box>
				))}
				<Pressable onPress={pickDocuments}>
					<AntDesign name="file-add" size={73} color="black" />
				</Pressable>
			</HStack>
			<TmobisButton
				label="Добавить"
				onPress={pickDocuments}
			/>
		</>
	);
};

export { FileComponent };