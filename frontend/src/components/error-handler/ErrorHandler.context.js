import { createInjectionState, useConfirmDialog } from '@vueuse/core';

const [provideErrorHandler, injectErrorHandler] = createInjectionState(useConfirmDialog);

export { provideErrorHandler };

export function useErrorHandler() {
  const context = injectErrorHandler();

  if (!context) {
    throw new Error(
      'Please call `provideErrorHandler` on the appropriate parent component',
    );
  }

  return context;
}