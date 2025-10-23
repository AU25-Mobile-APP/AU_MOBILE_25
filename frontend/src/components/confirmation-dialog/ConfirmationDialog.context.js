import { createInjectionState, useConfirmDialog } from '@vueuse/core';

const [provideConfirmationDialog, injectConfirmationDialog] = createInjectionState(useConfirmDialog);

export { provideConfirmationDialog };

export function useConfirmationDialog() {
  const context = injectConfirmationDialog();

  if (!context) {
    throw new Error(
      'Please call `provideConfirmationDialog` on the appropriate parent component',
    );
  }

  return context;
}
