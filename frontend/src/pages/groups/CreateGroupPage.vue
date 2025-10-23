<template>
  <v-row>
    <v-col offset-lg="3" sm="12" lg="6">
      <v-form @submit.prevent="onSubmit">
        <v-card
          :title="t('messages.createGroup')"
          :subtitle="t('messages.createGroupSubtitle')"
        >
          <template #text>
            <v-row dense>
              <v-col>
                <Field
                  v-slot="{ componentField, errors }"
                  name="name"
                  rules="required|alpha_num"
                >
                  <v-text-field
                    v-bind="componentField"
                    :error-messages="errors"
                    :label="t('messages.name')"
                    type="text"
                    required
                  />
                </Field>
              </v-col>
            </v-row>
            <v-row>
              <v-col>
                <Field
                  v-slot="{ componentField, errors }"
                  name="locationRange"
                  :rules="{
                    required: true,
                    max_value: 10000,
                    min_value: 100
                  }"
                >
                  <v-text-field
                    v-bind="componentField"
                    :error-messages="errors"
                    :label="t('messages.locationRange')"
                    type="text"
                    required
                  />
                </Field>
              </v-col>
            </v-row>

            <v-row dense>
              <v-col cols="12">
                <span class="text-overline">{{ t('messages.location') }}</span>
              </v-col>
              <v-col>
                <div>
                  <GMapMap
                    :center="center"
                    :zoom="14"
                    map-type-id="hybrid"
                    :options="{
                      zoomControl: true,
                      mapTypeControl: false,
                      scaleControl: false,
                      streetViewControl: false,
                      rotateControl: false,
                      fullscreenControl: false,
                    }"
                    class="maps"
                    @click="updateMarkerAfterEvent"
                  >
                    <GMapMarker
                      v-if="marker"
                      :position="marker"
                      draggable
                      @dragend="updateMarkerAfterEvent"
                    />
                    <GMapCircle
                      v-if="marker && controlledValues.locationRange"
                      :radius="controlledValues.locationRange * 1"
                      :center="marker"
                    />
                  </GMapMap>
                </div>
              </v-col>
            </v-row>
            <!-- TODO: Own Component and a bit more beutiful if time -->
            <v-row>
              <v-col>
                <p :class="`text-h6`">{{ t('messages.filter') }}</p>
                <span v-if="isPending">{{ t('messages.query.loading') }}</span>
                <span v-else-if="isError">{{ t('messages.query.error') }}</span>
                <div v-else>
                  <v-checkbox
                    v-for="filter in data"
                    :key="filter.id"
                    :label="t('messages.filterLabels.' + filter.id)"
                    hide-details
                    v-model="selectedFilters"
                    :value="filter.id"
                  />
                </div>
              </v-col>
            </v-row>
          </template>
          <template #actions>
            <v-btn
              color="error"
              :text="t('messages.cancel')"
              variant="flat"
              @click="router.push({ name: 'Home' })"
            />
            <v-btn
              color="primary"
              :text="t('messages.create')"
              variant="flat"
              type="submit"
              :loading="loading"
            />
          </template>
        </v-card>
      </v-form>
    </v-col>
  </v-row>
</template>

<script setup>
import { useDefaultFilters } from '@/api/filters';
import { createGroup } from '@/api/group';
import { useErrorHandler } from '@/components/error-handler/ErrorHandler.context';
import { useForm, Field } from 'vee-validate';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

const { t } = useI18n();
const { reveal } = useErrorHandler();

const router = useRouter();
const selectedFilters = ref([]);
const loading = ref(false);
const { isPending, isError, data } = useDefaultFilters();

const { handleSubmit, controlledValues } = useForm();

const center = ref({ lat: 47.4979134, lng: 8.726409 });

const marker = ref({ lat: 47.4979134, lng: 8.726409 });

function updateMarkerAfterEvent(newPositon) {
  marker.value = {
    lat: newPositon.latLng.lat(),
    lng: newPositon.latLng.lng()
  };
}

const onSubmit = handleSubmit(async (data) => {
  loading.value = true;
  try {
    const result = await createGroup({
      ...data,
      filters: selectedFilters.value.map((filter) => {
        return {
          id: filter,
          active: true
        };
      }),
      latitude: marker.value.lat,
      longitude: marker.value.lng
    });
    loading.value = false;
    router.push({ name: 'ViewGroup', params: { id: result } });
  } catch(error) {
    loading.value = false;
    reveal({ message: t('messages.error') });
  }
});

</script>

<style scoped>
.maps {
  height: 500px;
}

</style>
