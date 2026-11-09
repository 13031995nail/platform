import { Table } from '../ui/table';
import EntityListBody from './entity-list-body';
import EntityListFooter from './entity-list-footer';
import EntityListHeader from './entity-list-header';
import { ScrollView, StyleSheet } from 'react-native';

type EntityListViewProps = {
  headerSlot?: React.ReactNode;
};

export default function EntityListView({ headerSlot }: EntityListViewProps) {
  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      {headerSlot}
      <Table className="w-full">
        <EntityListHeader />
        <EntityListBody />
        <EntityListFooter />
      </Table>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    overflowX: "auto",
  },
  content: {
    paddingBottom: 16, 
  },
});