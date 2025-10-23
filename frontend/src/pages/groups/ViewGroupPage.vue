<template>
  <transition name="fade" mode="out-in">
    <v-row>
      <v-col
        v-if="isLoading || isRefetching"
        align="center"
        justify="center">
        <v-progress-circular
          
          color="primary"
          indeterminate
        />
      </v-col>
      <v-col v-if="!isLoading && !isRefetching">
        <v-row>
          <v-col cols="12" lg="auto">
            <h1>{{ group?.name }}</h1>
          </v-col>
          <v-spacer />
          <v-col cols="auto">
            <v-btn
              color="warning"
              variant="flat"
              :icon="mdiNumeric"
              @click="openCodeDialog = true"
            />
          </v-col>
          <v-col cols="auto">
            <v-btn
              color="pink"
              variant="flat"
              :icon="mdiFilterVariant"
              @click="openChangeFiltersDialog = true"
            />
          </v-col>
          <v-col cols="auto">
            <v-btn
              color="primary"
              variant="flat"
              :icon="mdiFormatListNumbered"
              @click="showRanking"
            />
          </v-col>
          <v-col cols="auto">
            <v-btn
              variant="flat"
              color="error"
              class="h-100"
              :text="t('messages.leaveGroup')"
              :prepend-icon="mdiAccountMultipleMinus"
              @click="leaveGroupAction"
            />
          </v-col>
        </v-row>

        <v-row>
          <v-col>
            <span v-if="isError">{{ t('messages.query.error') }}</span>
            <restaurant-details v-else :restaurants="restaurants" />
          </v-col>
        </v-row>

        <show-code-dialog v-model="openCodeDialog" />
        <change-filters-dialog v-model="openChangeFiltersDialog" @filters-changed="refetch"/>
      </v-col>
    </v-row>
  </transition>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { mdiAccountMultipleMinus, mdiFilterVariant, mdiFormatListNumbered, mdiNumeric } from '@mdi/js';
import ShowCodeDialog from '@/components/groups/ShowCodeDialog.vue';
import ChangeFiltersDialog from '@/components/groups/ChangeFiltersDialog.vue';
import { useConfirmationDialog } from '@/components/confirmation-dialog/ConfirmationDialog.context';
import { useRouter } from 'vue-router';
import { leaveGroup, useGroup, useGroupIdStore } from '@/api/group';
import { useRestaurants } from '@/api/restaurant';
import RestaurantDetails from '@/components/restaurant/RestaurantDetails.vue';
import { useRestaurantsStore } from '@/stores/restaurants';
import { storeToRefs } from 'pinia';
import { useErrorHandler } from '@/components/error-handler/ErrorHandler.context';

const { t } = useI18n();

const router = useRouter();

const openCodeDialog = ref(false);
const openChangeFiltersDialog = ref(false);

const { reveal } = useConfirmationDialog();
const { reveal: revealError } = useErrorHandler();

const { data: group } = useGroup();

const groupId = useGroupIdStore();

const { isError, refetch, isRefetching, isLoading } = useRestaurants();
const restaurantsStore  = useRestaurantsStore();
const { restaurants, groupId: storeGroupId } = storeToRefs(restaurantsStore);

onMounted(() => {
  if (groupId.value !== storeGroupId.value) {
    refetch();
  }
});

async function leaveGroupAction() {
  try {
    const { isCanceled } = await reveal({
      title: t('messages.leaveGroup'),
      message: t('messages.doYouWantToLeaveGroup')
    });

    if (!isCanceled) {
        await leaveGroup();
        router.push({ name: 'Home' });
    }
  } catch (error) {
    revealError({ message: t('messages.error') });
  }
}

function showRanking() {;
  router.push({ name: 'GroupRanking', params: { id: groupId.value} });
}

</script>
