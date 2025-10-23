<template>
  <v-row>
    <v-col>
      <v-form @submit.prevent="onSubmit">
        <v-card>
          <template #text>
            <Field
              v-slot="{ componentField, errors }"
              name="joinCode"
              rules="required|digits:8|min_value:10000000|max_value:99999999"
            >
              <v-text-field
                v-bind="componentField"
                :error-messages="errors"
                :label="t('messages.joinCode')"
                :placeholder="t('messages.enterJoinCode')"
                required
              />
            </Field>
          </template>
          <template #actions>
            <v-btn
              color="primary"
              variant="flat"
              :prepend-icon="mdiAccountMultiplePlus"
              class="w-100"
              :text="t('messages.joinGroup')"
              type="submit"
            />
          </template>
        </v-card>
      </v-form>
    </v-col>
  </v-row>
</template>

<script setup>
import { Field, useForm } from 'vee-validate';
import { useI18n } from 'vue-i18n';
import { mdiAccountMultiplePlus } from '@mdi/js';
import { joinGroup } from '@/api/group';
import { ApiError } from '@/api';
import router from '@/router';
import { useErrorHandler } from '../error-handler/ErrorHandler.context';
const { reveal } = useErrorHandler();

const { t } = useI18n();

const { handleSubmit } = useForm();

const onSubmit = handleSubmit(async (data) => {
    const code = data.joinCode;
    try {
        const id = await joinGroup(code);
        router.push({ name: 'ViewGroup', params: { id } });
    } catch(error) {
        console.log(error);
        if (!(error instanceof ApiError)) {
            return;
        }

        if (error.response.status === 404) {
            reveal({
                message: t('messages.groupNotExists')
            });
        }
    };
});
</script>
