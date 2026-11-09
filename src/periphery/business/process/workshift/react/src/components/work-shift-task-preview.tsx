import { useState, useEffect } from 'react';
import { View } from 'react-native';
import EntityList from '@tmobis/core/components/entity-list';
import useEntityList from '@tmobis/core/components/entity-list/use-entity-list';
import { TmobisIcon } from '@tmobis/core/components/icon';
import { TmobisModal } from '@tmobis/core/components/modal';
import { Pressable } from '@tmobis/core/components/ui/pressable';
import { inputIconStyle } from '@tmobis/core/components/ui/input';
import { WorkShift } from '../models/work-shift';
import {
    WorkShiftTask,
    WorkShiftTaskClassName,
} from '../models/work-shift-task';
import { WorkShiftTaskViewNames } from '../views';

interface WorkShiftTaskPreviewProps {
    data: WorkShift;
}

function WorkShiftTaskModalContent({ workShift }: { workShift: WorkShift }) {
    const tasksListData = useEntityList<WorkShiftTask>({
        entityType: WorkShiftTaskClassName,
        viewName: WorkShiftTaskViewNames.List,
        defaultFilter: { workShiftId: workShift.id },
        useUrlParams: false,
    });

    return (

        <View style={{ height: 384 }}>
            <EntityList
                listData={tasksListData}
                hasFilter={false}
                hasSearch={false}
                hasSelection={false}
                showToolbar={false}
                showDeleteMenuItemInRow={false}
                showEditMenuItemInRow={false}
                getRowConfig={() => ({
                    mobileCardTitleColumn: 'plannedStartTime',
                })}
                columns={[
                    {
                        column: 'plannedStartTime',
                        title: 'Плановое начало',
                    },
                    {
                        column: 'actualStartTime',
                        title: 'Фактическое начало',
                    },
                    {
                        column: 'actualEndTime',
                        title: 'Фактическое окончание',
                    },
                    {
                        column: 'workShiftStatus.name',
                        title: 'Статус',
                    },
                ]}
            />
        </View>
    );
}

/**
 * Компактный превью пунктов смены в ячейке списка.
 */
function WorkShiftTaskPreviewComponent({ data: workShift }: WorkShiftTaskPreviewProps) {
    const [isModalOpen, setIsModalOpen] = useState(false);

    useEffect(() => {
        setIsModalOpen(false);
    }, [workShift.id]);

    return (
        <>
            <Pressable
                onPress={() => setIsModalOpen(true)}
                className="h-8 w-8 items-center justify-center rounded-md bg-background-muted"
                accessibilityLabel="Предпросмотр пунктов"
            >
                <TmobisIcon name="InfoIcon" className={inputIconStyle({})} />
            </Pressable>

            <TmobisModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                title="Предпросмотр"
                size="lg"
            >
                {isModalOpen && <WorkShiftTaskModalContent workShift={workShift} />}
            </TmobisModal>
        </>
    );
}

export default WorkShiftTaskPreviewComponent;
