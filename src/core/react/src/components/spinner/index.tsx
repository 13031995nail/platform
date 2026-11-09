import { StyleSheet } from 'react-native';
import { View } from '../ui/view';
import { Spinner } from '../ui/spinner';
import { Text } from '../ui/text';
import { VStack } from '../ui/vstack';

interface TmobisSpinnerProps {
  label?: null | string;
}

const TmobisSpinner = ({ label }: TmobisSpinnerProps) => {
  return (
    <View style={styles.spinner}>
      <VStack space="md" className="items-center">
        <Spinner size="large" color="grey" />
        {label ? (
          <Text>{label}</Text>
        ) : null}
      </VStack>
    </View>
  );
};

const styles = StyleSheet.create({
  spinner: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    padding: 16
  },
});

export { TmobisSpinner };