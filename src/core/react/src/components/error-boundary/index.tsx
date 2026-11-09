import { Component, type ErrorInfo, type ReactNode } from 'react';
import { useQueryErrorResetBoundary } from '@tanstack/react-query';
import { Text } from "@tmobis/core/components/ui/text";
import { StyleSheet, View } from 'react-native';
import { TmobisButton } from '@tmobis/core/components/button';
import { isApiError } from '@tmobis/core/api/api-error';
import { useApiClient } from '@tmobis/core/api/api-client-context';
import { createMessageApi } from '@tmobis/core/api/services/message-api';
import type { ApiClient } from '@tmobis/core/api/create-api-client';
import { FrontendError } from './messages/frontend-error';

type ErrorBoundaryProps = {
    children?: ReactNode;
    onReset: () => void;
    client: ApiClient;
};

type ErrorBoundaryState = {
    hasError: boolean;
};

class ErrorBoundaryInner extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
    state = { hasError: false };

    static getDerivedStateFromError(): ErrorBoundaryState {
        return { hasError: true };
    }

    componentDidCatch(error: unknown, errorInfo: ErrorInfo) {
        const status = getErrorStatus(error);
        const message = error instanceof Error ? error.message : 'Unknown error';

        if (status === 404) {
            console.warn('Ошибка 404 (не отправлена):', message);
            return;
        }

        if (typeof status === 'number' && Number.isInteger(status)) {
            const data = {
                message,
                stack: error instanceof Error ? error.stack ?? '' : '',
                component: errorInfo.componentStack ?? '',
            };

            const frontendError = new FrontendError(data);
            createMessageApi(this.props.client).sendMessage(frontendError)
                .then(() => console.log('Отправлено (статус:', status, ')'))
                .catch(err => console.error('Ошибка отправки:', err));
        } else {
            console.warn('Ошибка без статуса (не отправлена):', message);
        }
    }

    handleClose = () => {
        this.props.onReset();
        this.setState({ hasError: false });
    };

    render() {
        if (this.state.hasError) {
            return (
                <View style={styles.container}>
                    <Text>Что-то пошло не так.</Text>
                    <TmobisButton label="ОК" action="primary" onPress={this.handleClose} />
                </View>
            );
        }

        return this.props.children;
    }
}

const getErrorStatus = (error: unknown): number | undefined => {
    if (isApiError(error) && typeof error.status === 'number') {
        return error.status;
    }

    if (typeof error === 'object' && error !== null && 'status' in error) {
        const status = (error as { status?: unknown }).status;
        return typeof status === 'number' ? status : undefined;
    }

    return undefined;
};

const ErrorBoundary = ({ children }: { children?: ReactNode }) => {
    const client = useApiClient();
    const { reset } = useQueryErrorResetBoundary();
    return (
        <ErrorBoundaryInner onReset={reset} client={client}>
            {children}
        </ErrorBoundaryInner>
    );
};

const styles = StyleSheet.create({
    container: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center',
    },
});

export default ErrorBoundary;
