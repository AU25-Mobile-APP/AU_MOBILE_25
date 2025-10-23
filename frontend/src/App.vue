<template>
  <v-app>
    <router-view />

    <confirmation-dialog />
    <error-handler />
  </v-app>
</template>

<script setup>
import { RouterView } from 'vue-router';
import ConfirmationDialog from './components/confirmation-dialog/ConfirmationDialog.vue';
import ErrorHandler from './components/error-handler/ErrorHandler.vue';
import { provideConfirmationDialog } from './components/confirmation-dialog/ConfirmationDialog.context';
import { provideErrorHandler } from './components/error-handler/ErrorHandler.context';

provideConfirmationDialog();
provideErrorHandler();

</script>
