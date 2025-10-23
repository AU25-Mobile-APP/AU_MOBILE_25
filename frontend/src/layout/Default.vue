<template>
  <div>
    <!-- Main App Bar -->
    <v-app-bar :elevation="2">
      <v-app-bar-title>
        <span class="cursor-pointer" @click="router.push({ name: 'Home' })">
          {{ t('messages.swipeFeast') }}
        </span>
      </v-app-bar-title>
    </v-app-bar>


    <!-- Main Application -->
    <v-main class="d-flex">
      <v-container class="pt-3">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </v-container>
    </v-main>

    <cookie-banner />
  </div>
</template>
<script setup>
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';
import cookieBanner from '@/components/cookies/banner.vue';

const router = useRouter();
const { t } = useI18n();
</script>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
